using System;
using System.Collections.Generic;
using System.Linq;
using Bumblebee.Exceptions;
using Bumblebee.Interfaces;
using OpenQA.Selenium;

namespace Bumblebee.Extensions
{
    public static class Verification
    {
        /// <summary>
        /// Verification method that allows for passing a predicate expression to evaluate some condition and a message to display if predicate is not true.
        /// </summary>
        /// <remarks>
        /// When throwing an error on verification, the system will add "Unable to verify " to anything that you pass as a message.  The recommendation is that you 
        /// write your verification strings starting with "that".  An example verification of "that string is empty." would return "Unable to verify that string is empty."
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="verification"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Verify<T>(this T obj, string verification, Predicate<T> predicate)
        {
            if (!predicate(obj))
                throw new VerificationException("Unable to verify " + verification);

            return obj;
        }

        /// <summary>
        /// Verification method that allows for passing a predicate expression to evaluate some condition.
        /// </summary>
        /// <remarks>
        /// If the predicate fails, the system will throw a verification exception with the message "Unable to verify custom verification."
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static T Verify<T>(this T obj, Predicate<T> predicate)
        {
            return obj.Verify("custom verification.", predicate);
        }

        /// <summary>
        /// Verification method that allows for passing an assertion from any assertion library.
        /// </summary>
        /// <remarks>
        /// The message that is thrown from the assertion library that you use will be captured in the VerificationException.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="assertion"></param>
        /// <returns></returns>
        public static T VerifyThat<T>(this T value, Action<T> assertion)
        {
            try
            {
                assertion(value);
                return value;
            }
            catch (Exception ex)
            {
                throw new VerificationException(string.Format("Unable to verify.\r\n{0}", ex.Message), ex);
            }
        }

        public static TSelectable VerifySelected<TSelectable>(this TSelectable selectable, bool selected) where TSelectable : ISelectable
        {
            if (selectable.Selected != selected)
                throw new VerificationException("Selection verification failed. Expected: " + selected + ", Actual: " + selectable.Selected + ".");

            return selectable;
        }

        public static THasText VerifyText<THasText>(this THasText hasText, string text) where THasText : IHasText
        {
            if (hasText.Text != text)
                throw new VerificationException("Text verification failed. Expected: " + text + ", Actual: " + hasText.Text + ".");
            return hasText;
        }

        public static THasText VerifyTextMismatch<THasText>(this THasText hasText, string text) where THasText : IHasText
        {
            if (hasText.Text == text)
                throw new VerificationException("Text mismatch verification failed. Unexpected: " + text + ", Actual: " + hasText.Text + ".");
            return hasText;
        }

        public static THasText VerifyTextContains<THasText>(this THasText hasText, string text)
            where THasText : IHasText
        {
            if (!hasText.Text.Contains(text))
                throw new VerificationException("Expected \"" + hasText.Text + "\" to contain \"" + text + "\"");
            return hasText;
        }

        public static TBlock VerifyPresence<TBlock>(this TBlock block, By by) where TBlock : IBlock
        {
            return block.VerifyPresenceOf("element", by);
        }

        public static TBlock VerifyAbsence<TBlock>(this TBlock block, By by) where TBlock : IBlock
        {
            return block.VerifyAbsenceOf("element", by);
        }

        public static TBlock VerifyPresenceOf<TBlock>(this TBlock block, string element, By by) where TBlock : IBlock
        {
            if (!block.Tag.GetElements(by).Any())
                throw new VerificationException("Couldn't verify presence of " + element + " " + by);

            return block;
        }

        public static TBlock VerifyAbsenceOf<TBlock>(this TBlock block, string element, By by) where TBlock : IBlock
        {
            if (block.Tag.GetElements(by).Any())
                throw new VerificationException("Couldn't verify absence of " + element + " " + by);

            return block;
        }

        public static TElement VerifyClasses<TElement>(this TElement block, IEnumerable<string> expectedClasses) where TElement : IElement
        {
            block.Tag.VerifyClasses(expectedClasses);

            return block;
        }

        public static TElement VerifyClasses<TElement>(this TElement block, params string[] expectedClasses) where TElement : IElement
        {
            return block.VerifyClasses((IEnumerable<string>) expectedClasses);
        }

        public static void VerifyClasses(this IWebElement element, IEnumerable<string> expectedClasses)
        {
            var classes = element.GetClasses();

            var missingClasses = expectedClasses.Where(expected => !classes.Contains(expected)).ToList();

            if (missingClasses.Any())
            {
                var message = "Block is missing the following expected classes: ";
                message += missingClasses.Aggregate((current, missingClass) => current + ", " + missingClass);
                throw new VerificationException(message);
            }
        }

        public static void VerifyClasses(this IWebElement element, params string[] expectedClasses)
        {
            element.VerifyClasses((IEnumerable<string>) expectedClasses);
        }

        public static TBlock Store<TBlock, TData>(this TBlock block, out TData data, Func<TBlock, TData> exp)
        {
            data = exp(block);
            return block;
        }
    }
}