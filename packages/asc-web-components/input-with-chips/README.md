# InputWithChips

Custom input-with-chips

### Usage

```js
import InputWithChips from "@appserver/components/input-with-chips";
```

```jsx
<InputWithChips options={options} placeholder="Type your chips..." />
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
- By double-clicking on the mouse button on a specific selected chip, you can switch to the chip editing mode.
- You can exit the editing mode by pressing Escape, Enter or NumpadEnter.
- Remove the chips by clicking on the button in the form of a cross.
- Click on the chip once, thereby highlighting it.
- Hold down the shift button by moving the arrows to the left, right or clicking the mouse on the chips, thereby highlighting several chips.
- The highlighted chip(s) can be removed by clicking on the button Backspace.
- The selected chip(s) can be copied to the clipboard by pressing "ctrl + c".

### Properties

| Props         |      Type      | Required | Values |         Default          | Description                                               |
| ------------- | :------------: | :------: | :----: | :----------------------: | --------------------------------------------------------- |
| `options`     | `obj`, `array` |    ✅    |   -    |            -             | Array of objects with chips                               |
| `placeholder` |    `string`    |    -     |   -    | Add placeholder to props | The placeholder is displayed only when the input is empty |
