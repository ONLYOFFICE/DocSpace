# GroupButtonsMenu

Menu for group actions on a page.

### Usage

```js
import GroupButtonsMenu from "@docspace/components/group-buttons-menu";
```

```jsx
const menuItems = [
  {
    label: "Select",
    isDropdown: true,
    isSeparator: true,
    isSelect: true,
    fontWeight: "bold",
    children: [
      <DropDownItem key="aaa" label="aaa" />,
      <DropDownItem key="bbb" label="bbb" />,
      <DropDownItem key="ccc" label="ccc" />,
    ],
    onSelect: (a) => console.log(a),
  },
  {
    label: "Menu item 1",
    disabled: false,
    onClick: () => console.log("Menu item 1 action"),
  },
  {
    label: "Menu item 2",
    disabled: true,
    onClick: () => console.log("Menu item 2 action"),
  },
];
```

```jsx
<GroupButtonsMenu
  checked={false}
  menuItems={menuItems}
  visible={false}
  moreLabel="More"
  closeTitle="Close"
  onClose={() => console.log("Close action")}
  onChange={() => console.log("Toggle action")}
  selected={menuItems[0].label}
/>
```

### Properties

| Props        |    Type    | Required | Values | Default  | Description                      |
| ------------ | :--------: | :------: | :----: | :------: | -------------------------------- |
| `checked`    |   `bool`   |    -     |   -    | `false`  | Sets initial value of checkbox   |
| `selected`   |  `string`  |    -     |   -    | `Select` | Selected header value            |
| `menuItems`  |  `array`   |    -     |   -    |    -     | Button collection                |
| `visible`    |   `bool`   |    -     |   -    |    -     | Sets menu visibility             |
| `moreLabel`  |  `string`  |    -     |   -    |  `More`  | Label for more button            |
| `closeTitle` |  `string`  |    -     |   -    | `Close`  | Title for close menu button      |
| `onClick`    | `function` |    -     |   -    |    -     | onClick action on GroupButton`s  |
| `onClose`    | `function` |    -     |   -    |    -     | onClose action if menu closing   |
| `onChange`   | `function` |    -     |   -    |    -     | onChange action on use selecting |
