# InputWithChips

Custom input-with-chips

### Usage

```js
import InputWithChips from "@appserver/components/input-with-chips";
```

```jsx
<InputWithChips
  options={[]}
  onChange={(selected) => console.log(selected)}
  placeholder="Invite people by name or email"
  existEmailText="This email address has already been entered"
  invalidEmailText="Invalid email address"
/>
```

#### Options - an array of objects that contains the following fields:

```js
const options = [
  {
    label: "Ivan Petrov",
    value: "myname@gmul.com",
  },
];
```

Options have options:

- label - Display text
- value - Email address

#### Actions that can be performed on chips and input:

- Enter a chip into the input (chips are checked for a valid email, and the same chips).
- Add chips by pressing Enter or NumpadEnter.
- By double-clicking on the mouse button or pressing enter on a specific selected chip, you can switch to the chip editing mode.
- You can exit the editing mode by pressing Escape, Enter or NumpadEnter.
- Remove the chips by clicking on the button in the form of a cross.
- Click on the chip once, thereby highlighting it.
- Hold down the shift button by moving the arrows to the left, right or clicking the mouse on the chips, thereby highlighting several chips.
- The highlighted chip(s) can be removed by clicking on the button Backspace or Delete.
- The selected chip(s) can be copied to the clipboard by pressing "ctrl + c".
- You can remove all chips by clicking on the button "Clear list".

### Properties

| Props              |      Type      | Required | Values |                   Default                   | Description                                        |
| ------------------ | :------------: | :------: | :----: | :-----------------------------------------: | -------------------------------------------------- |
| `options`          | `obj`, `array` |    -     |   -    |                      -                      | Array of objects with chips                        |
| `placeholder`      |    `string`    |    -     |   -    |       Invite people by name or email        | Placeholder text for the input                     |
| `onChange`         |     `func`     |    ✅    |   -    |                      -                      | Will be called when the selected items are changed |
| `existEmailText`   |    `string`    |    -     |   -    | This email address has already been entered | Warning text when entering an existing email       |
| `invalidEmailText` |    `string`    |    -     |   -    |            Invalid email address            | Warning text when entering an invalid email        |
