// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using TestMethodDisplay = Xunit.Sdk.TestMethodDisplay;

namespace Stress.Framework
{
    public abstract class StressTestCaseBase : XunitTestCase
    {
        private static readonly string IMetricCollectorTypeInfoName =
            new ReflectionTypeInfo(typeof(IStressMetricCollector)).Name;

        public StressTestCaseBase(
            string testApplicationName,
            string variation,
            IMethodInfo warmupMethod,
            IMessageSink diagnosticMessageSink,
            ITestMethod testMethod,
            object[] testMethodArguments)
            : base(diagnosticMessageSink, TestMethodDisplay.Method, testMethod, null)
        {
            // Override display name to avoid getting info about TestMethodArguments in the
            // name (this is covered by the concept of Variation for Stress)
            var name = TestMethod.Method.GetCustomAttributes(typeof(FactAttribute))
                .First()
                .GetNamedArgument<string>("DisplayName") ?? BaseDisplayName;

            TestApplicationName = testApplicationName;
            TestMethodName = name;
            WarmupMethod = warmupMethod;
            DisplayName = $"{name} [Variation: {variation}]";

            DiagnosticMessageSink = diagnosticMessageSink;
            Variation = variation;

            var potentialMetricCollector = testMethod.Method.GetParameters().FirstOrDefault();

            if (potentialMetricCollector != null && IsMetricCollector(potentialMetricCollector.ParameterType))
            {
                var methodArguments = new List<object> { MetricCollector };
                if (testMethodArguments != null)
                {
                    methodArguments.AddRange(testMethodArguments);
                }

                TestMethodArguments = methodArguments.ToArray();
            }
            else
            {
                TestMethodArguments = testMethodArguments;
            }
        }

        public string TestApplicationName { get; }
        protected IMessageSink DiagnosticMessageSink { get; }
        public abstract IStressMetricCollector MetricCollector { get; protected set; }
        public string TestMethodName { get; protected set; }
        public string Variation { get; protected set; }
        public IMethodInfo WarmupMethod { get; protected set; }

        protected override string GetSkipReason(IAttributeInfo factAttribute) => EvaluateSkipConditions(TestMethod) ?? base.GetSkipReason(factAttribute);

        private string EvaluateSkipConditions(ITestMethod testMethod)
        {
            var conditionAttributes = testMethod.Method
                .GetCustomAttributes(typeof(ITestCondition))
                .OfType<ReflectionAttributeInfo>()
                .Select(attributeInfo => attributeInfo.Attribute)
                .ToList();

            conditionAttributes.AddRange(testMethod.TestClass.Class
                .GetCustomAttributes(typeof(ITestCondition))
                .OfType<ReflectionAttributeInfo>()
                .Select(attributeInfo => attributeInfo.Attribute));

            var reasons = conditionAttributes.Cast<ITestCondition>()
                .Where(condition => !condition.IsMet)
                .Select(condition => condition.SkipReason)
                .ToList();

            return reasons.Count > 0 ? string.Join(Environment.NewLine, reasons) : null;
        }

        protected override string GetUniqueID()
        {
            return $"{TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name}{TestMethod.TestClass.Class.Name}{TestMethod.Method.Name}{Variation}";
        }

        private static bool IsMetricCollector(ITypeInfo typeInfo)
        {
            if (string.Equals(typeInfo.Name, IMetricCollectorTypeInfoName, StringComparison.Ordinal))
            {
                return true;
            }

            foreach (var intrfc in typeInfo.Interfaces)
            {
                if (IsMetricCollector(intrfc))
                {
                    return true;
                }
            }

            if ((typeInfo.BaseType as ReflectionTypeInfo).Type != null)
            {
                return IsMetricCollector(typeInfo.BaseType);
            }

            return false;
        }
    }
}
