# FinanceTracker — Screen and UI Specification

## 1. Project Overview

**FinanceTracker** is an offline-first Android finance tracking application built with .NET MAUI, C#, and SQLite. It tracks personal income, personal expenses, and expenses covered by external funding sources such as employers.

The application must work without an internet connection and store financial records locally on the device.

### Core objectives

- Track genuine income received into the user's personal finances.
- Record personal expenses paid from the user's own funds.
- Record expenses covered by external sources, such as employers.
- Identify which organization covered each externally funded expense.
- View financial summaries over a selected period.
- Manage accounts, funding sources, and transaction categories.
- Delete incorrect transactions and re-enter them instead of editing them.
- Preserve historical records when accounts, categories, or organizations are deactivated.

## 2. Application Navigation

The main navigation uses a bottom tab bar with five primary screens.

| Tab | Screen | Purpose |
|---|---|---|
| Home | Dashboard | Show a financial overview and recent transactions. |
| Accounts | Accounts management | Manage the Personal and External accounts. |
| Sources | Organizations | Manage employers and other external funding sources. |
| Categories | Categories management | Manage income and expense categories. |
| Reports | Financial reports | Summarize income, expenses, and externally funded expenses. |

Additional screens are accessed from the main navigation or relevant actions:

- **Transactions:** View all recorded transactions.
- **Income:** Record genuine personal income.
- **Expense:** Record personal or externally funded expenses.

The Home screen should provide navigation to the full transaction list and the income and expense entry screens.

---

## 3. Home Screen — Dashboard

### Purpose

The Home screen is the application's primary overview. It should allow the user to understand their current financial position and access common actions quickly.

### Information to display

The dashboard should summarize the user's financial activity using clear, readable figures.

Recommended summary values:

- **Genuine income:** Total normal income received into the Personal account.
- **Personal expenses:** Total expenses paid from the Personal account.
- **Externally funded expenses:** Total expenses covered by external sources.
- **Total funds:** Genuine income plus externally funded expenses.
- **Personal net:** Genuine income minus personal expenses.

These totals should use the application's financial rules described later in this document.

### Recent transactions

Display a list of recent transactions, with useful details such as:

- Transaction description or title.
- Amount in UGX.
- Transaction category, where applicable.
- Transaction date.
- Relevant funding source or organization, where applicable.
- Transaction type.

Provide an action to open the complete transaction list.

### Primary actions

The dashboard should provide convenient access to:

- Record income.
- Record an expense.
- View all transactions.
- View financial reports.

### UI expectations

- Use clearly distinguishable summary figures.
- Format monetary values as whole Ugandan shillings.
- Avoid cluttering the screen with every available transaction.
- Display a useful empty state when no transactions exist.
- Refresh the dashboard after transactions are added or deleted.

**Implementation note:** The exact dashboard card layout and current summary fields should be confirmed against the existing implementation before redesigning them.

---

## 4. Accounts Screen

### Purpose

The Accounts screen manages the accounts used to classify where money comes from and where it is used.

### Supported accounts

The application uses exactly two account types:

1. **Personal**
   - Represents the user's own money.
   - Receives genuine personal income.
   - Pays personal expenses.

2. **External**
   - Represents funds associated with externally covered expenses.
   - Is used when recording expenses covered by employers or other external sources.
   - Helps separate externally funded activity from the user's personal spending.

There should be one account of each type, not multiple accounts of the same type.

### Information to display

Each account should display:

- Account name.
- Account type.
- Active or inactive status.

If balances are displayed, they must be derived consistently from the application's transaction and ledger rules. Do not invent a balance calculation or present external funding as personal income.

### Available actions

- Rename an account.
- Activate an account.
- Deactivate an account.

### Rules

- Account identity must remain consistent with its type.
- Existing transactions must retain their historical account references.
- Deactivating an account must not delete its history.
- An inactive account should not be available for new transactions.
- Since the application expects one Personal and one External account, the UI should not offer unrestricted account creation.

**Important design consideration:** Deactivating either of the two accounts can prevent users from recording transactions of that type. The UI should communicate this consequence and avoid accidental deactivation.

---

## 5. Sources Screen — Organizations

### Purpose

The Sources screen manages organizations that cover expenses on the user's behalf.

Examples include:

- An employer.
- A company.
- Another organization responsible for covering a particular expense.

Organizations are used to identify **who covered an expense**, not to classify the expense itself.

### Information to display

Each organization should display:

- Organization name.
- Active or inactive status.

### Available actions

- Add an organization.
- Rename an organization.
- Deactivate an organization.
- Reactivate an organization.

### Form fields

When adding or renaming an organization:

- Organization name.

The name should be required and should not duplicate another organization's name, ignoring differences in capitalization or accidental surrounding whitespace.

### Rules

- Only active organizations should be available when recording new externally funded expenses.
- Existing transactions must retain their organization references after deactivation.
- Deactivation must not remove historical transactions or reporting data.
- Organizations must not be hard-deleted when they are referenced by financial history.

### Empty state

When no organizations exist, display a helpful message explaining that the user can add an employer or another external funding source before recording externally funded expenses.

---

## 6. Categories Screen

### Purpose

The Categories screen manages labels used to classify transactions.

Categories answer **what kind of transaction occurred**, while organizations answer **who covered an expense**.

### Supported category types

The underlying transaction model defines several transaction kinds, including Expense, Income, Transfer, Allowance, Settlement, and Adjustment.

For the current income and expense entry screens, category management should support:

- Income categories.
- Expense categories.

Do not assume that all other transaction kinds need category-management UI unless those workflows are implemented.

### Information to display

Each category should display:

- Category name.
- Category type: Income or Expense.
- Active or inactive status.

Categories should be grouped or filterable by type.

### Available actions

- Create a category.
- Rename a category.
- Deactivate a category.
- Reactivate a category.

### Form fields

- Category name.
- Category type.

### Rules

- Category names must be required.
- Duplicate category names should be prevented within the same category type.
- An Income category and an Expense category may have the same name if the application allows names to be unique only within each type.
- Deactivated categories must not appear in new transaction forms.
- Historical transactions must retain their category references after a category is deactivated.
- Categories must be deactivated rather than hard-deleted when historical transactions reference them.

### Interaction with transaction forms

The Income form must show only active Income categories.

The Expense form must show only active Expense categories.

Changing a category's active status should affect future transaction entry without altering past records.

---

## 7. Transactions Screen — All Transactions

### Purpose

The Transactions screen displays the complete list of recorded transactions. It provides a place to inspect financial activity and remove incorrect records.

### Information displayed per transaction

The current transaction list displays:

- **Title:** Transaction description or title.
- **Category:** The associated category, where available.
- **Date:** The date the transaction occurred.
- **Source name:** The associated source or organization, where available.
- **Kind:** The transaction type.
- **Amount:** The transaction amount formatted as UGX.

The current layout uses transaction cards with the main details on the left and the amount and Delete button on the right.

### Available actions

- View transaction details in the list.
- Delete a transaction.
- Navigate back to the dashboard or other screens.

The current design does not require transaction editing. To correct a transaction, delete the incorrect record and enter it again.

### Delete workflow

When the user taps Delete:

1. Display a confirmation dialog.
2. Identify the transaction by its title or description.
3. Explain that deletion cannot be undone.
4. Provide Delete and Cancel actions.
5. If the user cancels, make no changes.
6. If the user confirms, delete the transaction through the transaction service.
7. Remove the deleted transaction from the visible list only after deletion succeeds.
8. Display an error if deletion fails.

### Critical external funding rule

Externally funded expenses are represented by two linked records:

- An Expense transaction associated with the External account and the organization that covered it.
- A linked Income transaction used as an external funding offset.

These two records represent one externally funded expense and must not be treated as two independent financial activities.

When an externally funded expense is deleted:

- Delete the expense.
- Delete its linked funding-offset income.
- Perform both deletions in a single database transaction.
- Commit both changes together.
- If deletion fails, roll back the database transaction.

The offset-income record should not be independently deletable through the normal transaction list. The associated expense is the user-facing record.

### Refresh behavior

The transaction list should reload when the screen appears so that changes made elsewhere are reflected.

The Home screen and Reports screen should also reflect deleted transactions when their data is refreshed.

### Empty and error states

- If there are no transactions, display: "No transactions recorded yet."
- If loading fails, display a readable error message.
- If deletion fails, preserve the transaction in the list and communicate the failure.

---

## 8. Income Screen — Record Income

### Purpose

The Income screen records genuine income received into the user's Personal account.

Examples might include salary, payment for work, or another legitimate source of personal income.

### Information to collect

The form should support:

- **Amount:** The amount received, in UGX.
- **Category:** An active Income category.
- **Description:** A human-readable explanation of the income.
- **Date:** The date the income occurred.

### Behavior

When the user saves a valid income transaction:

1. Validate the amount.
2. Confirm that the selected category is active and is an Income category.
3. Use the active Personal account.
4. Create a posted Income transaction.
5. Save the transaction locally.
6. Provide clear feedback and update relevant financial summaries.

### Validation

- The amount must be valid and positive.
- A valid Income category must be selected.
- The transaction date must be valid.
- The description should be trimmed before saving.
- The transaction must not be recorded as an external funding offset.

### Financial treatment

Genuine income contributes to:

- Total genuine income.
- Total funds.
- Personal net.

It does not contribute to personal expenses or externally funded expenses.

### Offline behavior

Recording income must work without an internet connection.

---

## 9. Expense Screen — Record Expense

### Purpose

The Expense screen records expenses and identifies whether they were paid from the user's own money or covered by an external organization.

This distinction is fundamental to the application's reporting.

### Information to collect

The form should support:

- **Amount:** Expense amount in UGX.
- **Category:** An active Expense category.
- **Description:** A description of what the expense was for.
- **Date:** The date the expense occurred.
- **Funding type:** Personal or externally funded.
- **Organization:** Required when the expense is externally funded.

### Personal expense behavior

When the user selects personal funding:

1. Validate the amount and category.
2. Use the Personal account.
3. Create a posted Expense transaction.
4. Do not create an external funding-offset transaction.
5. Save the transaction locally.

A personal expense contributes to personal expenses and reduces personal net.

### Externally funded expense behavior

When the user selects external funding:

1. Validate the amount and category.
2. Require an active external funding organization.
3. Use the External account.
4. Create a posted Expense transaction linked to the selected organization.
5. Create a corresponding posted Income transaction as a funding offset.
6. Link the two transactions through their related transaction IDs.
7. Save both records atomically in one database transaction.

The funding-offset income exists to represent the external funding in the transaction model. It must not be counted as genuine personal income.

### Conditional UI behavior

- When Personal is selected, the organization selector should be hidden or disabled.
- When External is selected, show the organization selector.
- Only active organizations should be selectable.
- Only active Expense categories should be available regardless of funding type.

### Validation

- The amount must be valid and positive.
- An active Expense category must be selected.
- A valid funding type must be selected.
- Externally funded expenses require an active organization.
- The description should be trimmed before saving.
- The transaction date must be valid.

### Offline behavior

Expense entry and all related database writes must work offline.

---

## 10. Reports Screen

### Purpose

The Reports screen summarizes financial activity over a selected date range.

The existing report view model initializes the date range to the first day of the current month through the current date.

### Date range

The screen should provide:

- Start date.
- End date.
- An action to refresh or apply the selected range.

The date range should include the selected start and end dates. Internally, the query may implement this as a start-inclusive, end-exclusive interval by adding one day to the selected end date.

### Summary figures

#### Genuine income

The total of normal income received into the Personal account, excluding funding-offset income.

Formula:

`Genuine income = Sum of eligible personal income transactions`

#### Personal expenses

The total expenses paid from the Personal account.

Formula:

`Personal expenses = Sum of personal expense transactions`

#### Externally funded expenses

The total expenses covered by external organizations.

Formula:

`Externally funded expenses = Sum of expense transactions from the External account`

#### Total funds

The combined total of genuine income and externally funded expenses.

Formula:

`Total funds = Genuine income + Externally funded expenses`

This figure represents genuine income plus expenses covered by external sources. It is not the user's cash balance.

#### Personal net

The difference between genuine income and personal expenses.

Formula:

`Personal net = Genuine income - Personal expenses`

This is a period-based net figure, not necessarily the current account balance.

### External funding breakdown

The screen should display externally funded expenses grouped by organization.

Each organization entry should show:

- Organization name.
- Total externally funded expenses attributed to that organization during the selected period.

This allows the user to answer questions such as: "How much of my expenses did a particular employer cover during this month?"

### Reporting rules

- Include only posted transactions.
- Respect the selected date range.
- Exclude funding-offset income from genuine income.
- Count an externally funded expense once, not once as an expense and again as income.
- Preserve historical records associated with deactivated organizations and accounts.
- Use transaction dates rather than creation timestamps for period reporting.
- Refresh the report when the date range changes or when the user requests a refresh.

### Empty state

If there is no financial activity in the selected period, show zero-valued summaries and an appropriate message rather than an error.

---

## 11. Shared UI and Interaction Requirements

### Monetary formatting

- Currency: UGX.
- Amounts are stored as whole shillings.
- Display amounts with thousands separators.
- Do not introduce decimal currency values unless the financial model is deliberately changed.

Example: `UGX 125,000`

### Forms

- Clearly label every field.
- Indicate required fields.
- Display validation messages near the relevant field or in a visible form-level error area.
- Prevent duplicate submissions while a save is in progress.
- Give clear feedback when saving succeeds or fails.
- Preserve the user's entered values when validation fails, where practical.

### Loading and error states

Every screen that loads or saves data should handle:

- Loading state.
- Empty state.
- Validation errors.
- Database errors.
- Successful completion.

Errors should be understandable without exposing unnecessary technical details to the user.

### Visual consistency

- Use consistent spacing, typography, and button styling.
- Make amounts easy to scan.
- Distinguish destructive actions such as Delete from normal actions.
- Use confirmation dialogs for irreversible operations.
- Ensure the layout remains usable on typical Android phone screen sizes.
- Prefer a clean, straightforward financial dashboard over decorative UI.

### Offline-first behavior

- All core financial functionality must work offline.
- SQLite is the local source of truth.
- Do not make transaction creation, deletion, or reporting dependent on a network request.
- Database transactions must preserve consistency when multiple linked records are written or deleted.

---

## 12. Financial Domain Rules — Must Not Be Violated

These rules apply across all screens and services.

1. There is one Personal account and one External account.
2. Genuine income belongs to the Personal account.
3. Personal expenses use the Personal account.
4. Externally funded expenses use the External account and reference an organization.
5. Every externally funded expense has a linked funding-offset income record.
6. Funding-offset income must never be included in genuine income totals.
7. Deleting an externally funded expense must also delete its linked offset record atomically.
8. Deactivated categories and organizations remain available to historical records.
9. Only active categories and organizations are available for new applicable transactions.
10. Transactions are corrected by deletion and re-entry, not editing.
11. Reports must use transaction occurrence dates and the selected reporting period.
12. All monetary totals must avoid double-counting external funding.
13. Total funds means genuine income plus externally funded expenses; it does not mean account balance.
14. Personal net means genuine income minus personal expenses; it does not necessarily mean cash currently available.
15. Financial data must remain available offline.

---

## 13. Suggested Agent Workflow

When modifying the application, the coding agent should follow these rules:

1. Inspect the existing implementation before changing files.
2. Preserve working business logic and navigation.
3. Reuse existing services, models, and view models where appropriate.
4. Keep database operations inside services rather than implementing financial logic directly in XAML code-behind.
5. Avoid introducing duplicate calculations in multiple screens.
6. Use the report service as the shared source for report calculations.
7. Preserve the linked-transaction behavior for externally funded expenses.
8. Do not hard-delete categories, organizations, or accounts that have historical references.
9. Do not add transaction editing unless explicitly requested.
10. Avoid changing the database schema without checking migration and existing-data implications.
11. Build the application after changes using:

```bash
dotnet build -f net10.0-android
```

12. Test both ordinary and externally funded transactions, especially creation, deletion, date-range reporting, and historical records.

## 14. Details the Agent Must Verify Before Redesigning

The following details are not fully established by the screen code shared so far and should be inspected rather than assumed:

- The exact cards and figures currently displayed on Home.
- Whether Home already displays recent transactions and which fields it uses.
- Whether account balances are displayed and how they are calculated.
- The exact Income and Expense form layout and validation messages.
- Whether Reports currently displays all five summary figures and an organization breakdown.
- Whether transaction list results exclude funding-offset income.
- Whether the app has a separate transaction-detail screen.
- Whether category creation supports transaction kinds beyond Income and Expense.
- The current loading indicators, confirmation dialogs, and success messages on each screen.

**Primary instruction:** Treat this document as the functional specification for the existing application, not permission to replace working functionality wholesale. Inspect the current implementation, identify gaps, and make focused changes while preserving the financial rules and offline-first behavior.
