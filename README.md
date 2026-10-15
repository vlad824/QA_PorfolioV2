# QA Automation Portfolio — C# / .NET 8

Two test projects against public demo systems, structured the way a team codebase would be:

| Project | Stack | Target |
|---|---|---|
| `api-restsharp/` | NUnit + RestSharp | [Restful-Booker API](https://restful-booker.herokuapp.com/) |
| `ui-playwright/` | NUnit + Playwright (`PageTest`) | [Restful-Booker Platform UI](https://automationintesting.online/) |

## Quick start
```bash
dotnet test api-restsharp

dotnet build ui-playwright
pwsh ui-playwright/bin/Debug/net8.0/playwright.ps1 install chromium
dotnet test ui-playwright --settings ui-playwright/ui.runsettings
```
Run a subset: `dotnet test api-restsharp --filter "Category=Smoke"` (categories: `Smoke`, `Crud`, `Auth`, `Negative`, `ContactForm`).
Watch the browser: `HEADED=1 dotnet test ui-playwright`.

## Configuration (environment variables, all optional)
`BOOKER_BASE_URL`, `BOOKER_USERNAME`, `BOOKER_PASSWORD`, `BOOKER_TIMEOUT_MS`, `UI_BASE_URL`.
Defaults point to the public demos; the `admin` / `password123` pair is published in the API documentation.

## Design decisions
- **API client separated from tests** (`Clients/BookerClient`): no assertions inside, typed responses, one place for headers/auth.
- **Typed models as records** with explicit `JsonPropertyName` — payload-vs-response checks use value equality instead of field-by-field asserts.
- **Test data builder** (`BookingBuilder`): valid by default, unique names, tests override only what they care about.
- **Test isolation**: each test creates its own data and deletes it in `TearDown`; fixtures run in parallel safely.
- **Full CRUD lifecycle + negative paths**: create / read / filter / PUT / PATCH / DELETE, missing or invalid token (403), unknown ids (404), invalid payload, bad credentials.
- **Known API quirks are documented in code** (200 on bad login, 201 on DELETE, 500 on invalid payload) and assertions target the real requirement, not the bug.
- **Page Object + component** (`HomePage`, `ContactForm`), `GetByTestId` / role locators, web-first `Expect` assertions (auto-wait, no sleeps).
- **One browser context per test** via `PageTest`; Playwright **trace is saved only for failed tests** (`TestResults/traces/*.zip`).
- **Central config**: `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, one solution file.
- **CI**: GitHub Actions runs both suites and uploads TRX results and traces.

## Things to verify on first run
I could not run these tests against the live sites while preparing them, so check these assumptions first:
- Contact-form selectors (`ContactName`, `ContactEmail`, `ContactPhone`, `ContactSubject`, `ContactDescription` test ids, `#submitContact`) and message wording (`Thanks for getting in touch`, `must be between`, `may not be blank`) — all live in `Pages/ContactForm.cs`, so a fix is a one-file change.
- RestSharp 112 timeout option (`MaxTimeout` in `BookerClient`) — rename if your compiler flags it.

## Limitations
Both targets are shared public sandboxes: they can be slow, reset, or change. The suites are demonstration material, not evidence of commercial project experience — be ready to explain every assertion.
