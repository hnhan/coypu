using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Coypu.Actions;
using Coypu.Finders;
using Coypu.Queries;
using Coypu.Robustness;

namespace Coypu
{
    /// <summary>
    /// A browser window belonging to a particular browser session
    /// </summary>
    public class BrowserWindow : Scope<BrowserWindow>, IDisposable
    {
        protected readonly Driver driver;
        protected readonly Configuration configuration;
        protected readonly RobustWrapper robustWrapper;
        protected readonly UrlBuilder urlBuilder;

        internal bool WasDisposed { get; private set; }
        private readonly DriverScope driverScope;

        internal BrowserWindow(Driver driver, Configuration configuration, RobustWrapper robustWrapper, Waiter waiter,UrlBuilder urlBuilder)
        {
            this.driver = driver;
            driverScope = new DriverScope(configuration,new DocumentElementFinder(driver), driver, robustWrapper, waiter, urlBuilder);
            this.configuration = configuration;
            this.robustWrapper = robustWrapper;
            this.urlBuilder = urlBuilder;
        }

        internal DriverScope DriverScope
        {
            get { return driverScope; }
        }

        internal Driver Driver
        {
            get { return driver; }
        }

        /// <summary>
        /// The native driver for the Coypu.Driver / browser combination you supplied. E.g. for SeleniumWebDriver + Firefox it will currently be a OpenQA.Selenium.Firefox.FirefoxDriver.
        /// </summary>
        public object Native
        {
            get { return driver.Native; }
        }

        /// <summary>
        /// The current location of the browser
        /// </summary>
        public Uri Location
        {
            get { return DriverScope.Location; }
        }

        /// <summary>
        /// Disposes the current session, closing any open browser.
        /// </summary>
        public void Dispose()
        {
            if (WasDisposed)
                return;

            Console.WriteLine("Disposing driver");

            driver.Dispose();

            Console.WriteLine("Disposed");
            ActivatorDriverFactory.OpenDrivers--;
            Console.WriteLine(ActivatorDriverFactory.OpenDrivers + " driver(s) open.");

            WasDisposed = true;
        }

        /// <summary>
        /// Check that a dialog with the specified text appears within the <see cref="Configuration.Timeout"/>
        /// </summary>
        /// <param name="withText">Dialog text</param>
        /// <returns>Whether an element appears</returns>
        public bool HasDialog(string withText, Options options = null)
        {
            return Query(new HasDialogQuery(driver, withText, driverScope, driverScope.Default(options)));
        }

        /// <summary>
        /// Check that a dialog with the specified is not present. Returns as soon as the dialog is not present, or when the <see cref="Configuration.Timeout"/> is reached.
        /// </summary>
        /// <param name="withText">Dialog text</param>
        /// <returns>Whether an element does not appears</returns>
        public bool HasNoDialog(string withText, Options options = null)
        {
            return Query(new HasNoDialogQuery(driver, withText, driverScope, driverScope.Default(options)));
        }

        /// <summary>
        /// Accept the first modal dialog to appear within the <see cref="Configuration.Timeout"/>
        /// </summary>
        /// <exception cref="T:Coypu.MissingHtmlException">Thrown if the dialog cannot be found</exception>
        public void AcceptModalDialog(Options options = null)
        {
            driverScope.RetryUntilTimeout(new AcceptModalDialog(driverScope, driver, driverScope.Default(options)));
        }

        /// <summary>
        /// Cancel the first modal dialog to appear within the <see cref="Configuration.Timeout"/>
        /// </summary>
        /// <exception cref="T:Coypu.MissingHtmlException">Thrown if the dialog cannot be found</exception>
        public void CancelModalDialog(Options options = null)
        {
            driverScope.RetryUntilTimeout(new CancelModalDialog(driverScope, driver, driverScope.Default(options)));
        }

        public BrowserWindow ClickButton(string locator, Options options = null)
        {
            driverScope.ClickButton(locator,options);
            return this;
        }

        public BrowserWindow ClickLink(string locator, Options options = null)
        {
            driverScope.ClickLink(locator, options);
            return this;
        }

        public BrowserWindow ClickButton(string locator, Query<bool> until, TimeSpan waitBeforeRetry, Options options = null)
        {
            driverScope.ClickButton(locator, until, waitBeforeRetry, options);
            return this;
        }

        public BrowserWindow ClickLink(string locator, Query<bool> until, TimeSpan waitBeforeRetry, Options options = null)
        {
            driverScope.ClickLink(locator, until, waitBeforeRetry, options);
            return this;
        }

        /// <summary>
        /// Visit a url in the browser
        /// </summary>
        /// <param name="virtualPath">Virtual paths will use the Configuration.AppHost,Port,SSL settings. Otherwise supply a fully qualified URL.</param>
        public BrowserWindow Visit(string virtualPath)
        {
            driver.Visit(urlBuilder.GetFullyQualifiedUrl(virtualPath,configuration));
            return this;
        }

        public ElementScope FindButton(string locator, Options options = null)
        {
            return driverScope.FindButton(locator, options);
        }

        public ElementScope FindLink(string locator, Options options = null)
        {
            return driverScope.FindLink(locator, options);
        }

        public ElementScope FindField(string locator, Options options = null)
        {
            return driverScope.FindField(locator, options);
        }

        public FillInWith FillIn(string locator, Options options = null)
        {
            return driverScope.FillIn(locator, options);
        }

        public FillInWith FillIn(Element element, Options options = null)
        {
            return driverScope.FillIn(element, options);
        }

        public SelectFrom Select(string option, Options options = null)
        {
            return driverScope.Select(option, options);
        }

        public bool HasContent(string text, Options options = null)
        {
            return driverScope.HasContent(text, options);
        }

        public bool HasContentMatch(Regex pattern, Options options = null)
        {
            return driverScope.HasContentMatch(pattern, options);
        }

        public bool HasNoContent(string text, Options options = null)
        {
            return driverScope.HasNoContent(text, options);
        }

        public bool HasNoContentMatch(Regex pattern, Options options = null)
        {
            return driverScope.HasNoContentMatch(pattern, options);
        }

        public bool HasCss(string cssSelector, Options options = null)
        {
            return driverScope.HasCss(cssSelector, options);
        }

        public bool HasNoCss(string cssSelector, Options options = null)
        {
            return driverScope.HasNoCss(cssSelector, options);
        }

        public bool HasXPath(string xpath, Options options = null)
        {
            return driverScope.HasXPath(xpath, options);
        }

        public bool HasNoXPath(string xpath, Options options = null)
        {
            return driverScope.HasNoXPath(xpath, options);
        }

        public ElementScope FindCss(string cssSelector, Options options = null)
        {
            return driverScope.FindCss(cssSelector, options);
        }

        public ElementScope FindXPath(string xpath, Options options = null)
        {
            return driverScope.FindXPath(xpath, options);
        }

        public IEnumerable<ElementFound> FindAllCss(string cssSelector, Options options = null)
        {
            return driverScope.FindAllCss(cssSelector, options);
        }

        public IEnumerable<ElementFound> FindAllXPath(string xpath, Options options = null)
        {
            return driverScope.FindAllXPath(xpath, options);
        }

        public ElementScope FindSection(string locator, Options options = null)
        {
            return driverScope.FindSection(locator, options);
        }

        public IFrameElementScope FindIFrame(string locator, Options options = null)
        {
            return driverScope.FindIFrame(locator, options);
        }

        public ElementScope FindFieldset(string locator, Options options = null)
        {
            return driverScope.FindFieldset(locator, options);
        }

        public ElementScope FindId(string id, Options options = null)
        {
            return driverScope.FindId(id, options);
        }

        public BrowserWindow Check(string locator, Options options = null)
        {
            driverScope.Check(locator, options);
            return this;
        }

        public BrowserWindow Uncheck(string locator, Options options = null)
        {
            driverScope.Uncheck(locator, options);
            return this;
        }

        public BrowserWindow Choose(string locator, Options options = null)
        {
            driverScope.Choose(locator,options);
            return this;
        }

        public string ExecuteScript(string javascript)
        {
            return driverScope.ExecuteScript(javascript);
        }

        public bool Has(ElementScope findElement)
        {
            return driverScope.Has(findElement);
        }

        public bool HasNo(ElementScope findElement)
        {
            return driverScope.HasNo(findElement);
        }

        public void RetryUntilTimeout(Action action, Options options = null)
        {
            driverScope.RetryUntilTimeout(action, options);
        }

        public TResult RetryUntilTimeout<TResult>(Func<TResult> function, Options options = null)
        {
            return driverScope.RetryUntilTimeout(function, options);
        }

        public void RetryUntilTimeout(DriverAction driverAction)
        {
            driverScope.RetryUntilTimeout(driverAction);
        }

        public T Query<T>(Func<T> query, T expecting, Options options = null)
        {
            return driverScope.Query(query, expecting, options);
        }

        public T Query<T>(Query<T> query)
        {
            return driverScope.Query(query);
        }

        public void TryUntil(Action tryThis, Func<bool> until, TimeSpan waitBeforeRetry, Options options = null)
        {
            driverScope.TryUntil(tryThis, until, waitBeforeRetry, options);
        }

        public void TryUntil(DriverAction tryThis, Query<bool> until, TimeSpan waitBeforeRetry, Options options = null)
        {
            driverScope.TryUntil(tryThis, until, waitBeforeRetry, options);
        }

        public State FindState(params State[] states)
        {
            return DriverScope.FindState(states);
        }

        public State FindState(State[] states, Options options = null)
        {
            return DriverScope.FindState(states, options);
        }
    }
}