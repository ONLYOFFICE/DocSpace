# EmailChips

Custom email-chips

### Usage

```js
import EmailChips from "@docspace/components/email-chips";
```

```jsx
<EmailChips
  options={[]}
  onChange={(selected) => console.log(selected)}
  placeholder="Invite people by name or email"
  clearButtonLabel="Clear list"
  existEmailText="This email address has already been entered"
  invalidEmailText="Invalid email address"
  exceededLimitText="The limit on the number of emails has reached the maximum"
  exceededLimitInputText="The limit on the number of characters has reached the maximum value"
  chipOverLimitText="The limit on the number of characters has reached the maximum value"
  exceededLimit=500,
/>
```

#### Options - an array of objects that contains the following fields:

```js
const options = [
  {
    name: "Ivan Petrov",
    email: "myname@gmul.com",
    isValid: true,
  },
];
```

Options have options:

- name - Display text
- email - Email address
- isValid - Displays whether the email is valid

#### Actions that can be performed on chips and input:

- Enter a chip into the input (chips are checked for a valid email, and the same chips).
- Add chips by pressing Enter or NumpadEnter.
- By double-clicking on the mouse button or pressing enter on a specific selected chip, you can switch to the chip editing mode.
- You can exit the editing mode by pressing Escape, Enter, NumpadEnter or by clicking ouside.
- Remove the chips by clicking on the button in the form of a cross.
- Click on the chip once, thereby highlighting it.
- Hold down the shift button by moving the arrows to the left, right or clicking the mouse on the chips, thereby highlighting several chips.
- The highlighted chip(s) can be removed by clicking on the button Backspace or Delete.
- The selected chip(s) can be copied to the clipboard by pressing "ctrl + c".
- You can remove all chips by clicking on the button "Clear list".

### Properties

| Props                    |      Type      | Required | Values |                                     Default                                     | Description                                                                      |
| ------------------------ | :------------: | :------: | :----: | :-----------------------------------------------------------------------------: | -------------------------------------------------------------------------------- |
| `options`                | `obj`, `array` |    -     |   -    |                                        -                                        | Array of objects with chips                                                      |
| `onChange`               |     `func`     |    ✅    |   -    |                                        -                                        | displays valid email addresses. Called when changing chips                       |
| `placeholder`            |    `string`    |    -     |   -    |                         Invite people by name or email                          | Placeholder text for the input                                                   |
| `clearButtonLabel`       |    `string`    |    -     |   -    |                                   Clear list                                    | The text of the button for cleaning all chips                                    |
| `existEmailText`         |    `string`    |    -     |   -    |                   This email address has already been entered                   | Warning text when entering an existing email                                     |
| `invalidEmailText`       |    `string`    |    -     |   -    |                              Invalid email address                              | Warning text when entering an invalid email                                      |
| `exceededLimit`          |    `number`    |    -     |   -    |                                       500                                       | Limit of chips (number)                                                          |
| `exceededLimitText`      |    `string`    |    -     |   -    |            The limit on the number of emails has reached the maximum            | Warning text when exceeding the limit of the number of chips                     |
| `exceededLimitInputText` |    `string`    |    -     |   -    |       The limit on the number of characters has reached the maximum value       | Warning text when entering the number of characters in input exceeding the limit |
| `chipOverLimitText`      |    `string`    |    -     |   -    | The limit on the number of characters in an email has reached its maximum value | Warning text when entering the number of email characters exceeding the limit    |
