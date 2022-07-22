# GroupButton

Base Button is used for a group action on a page

### Usage

```js
import GroupButton from "@docspace/components/group-button";
```

```jsx
<GroupButton
  label="Group button"
  disabled={false}
  isDropdown={false}
  opened={false}
/>
```

It can be used as selector with checkbox for this following properties are combined: _isDropdown_, _isSeparator_.

_isSeparator_ will add vertical bar after button.

_isDropdown_ allows adding items to dropdown list in children.

For health of checkbox, button inherits part of properties of this component.

### Properties

| Props               |      Type      | Required | Values |    Default     | Description                                           |
| ------------------- | :------------: | :------: | :----: | :------------: | ----------------------------------------------------- |
| `action`            |     `func`     |    -     |   -    |       -        | What the button will trigger when clicked             |
| `checked`           |     `bool`     |    -     |   -    |    `false`     | Initial value of checkbox                             |
| `className`         |    `string`    |    -     |   -    |       -        | Accepts class                                         |
| `disabled`          |     `bool`     |    -     |   -    |    `false`     | Tells when the button should present a disabled state |
| `dropDownMaxHeight` |    `number`    |    -     |   -    |       -        | Selected height value of DropDown                     |
| `fontWeight`        |    `string`    |    -     |   -    |     `600`      | Value of font weight                                  |
| `id`                |    `string`    |    -     |   -    |       -        | Accepts id                                            |
| `isDropdown`        |     `bool`     |    -     |   -    |    `false`     | Tells when the button should present a dropdown state |
| `isIndeterminate`   |     `bool`     |    -     |   -    |    `false`     | Initial value of Indeterminate checkbox               |
| `isSeparator`       |     `bool`     |    -     |   -    |    `false`     | Tells when the button should contain separator        |
| `label`             |    `string`    |    -     |   -    | `Group button` | Value of the group button                             |
| `onChange`          |     `func`     |    -     |   -    |       -        | Called when checkbox is checked                       |
| `onClick`           |     `func`     |    -     |   -    |       -        | Property for onClick action                           |
| `onSelect`          |     `func`     |    -     |   -    |       -        | Called when value is selected in selector             |
| `opened`            |     `bool`     |    -     |   -    |    `false`     | Tells when the button should be opened by default     |
| `selected`          |    `string`    |    -     |   -    |       -        | Selected value label                                  |
| `style`             | `obj`, `array` |    -     |   -    |       -        | Accepts css style                                     |
| `tabIndex`          |    `number`    |    -     |   -    |      `-1`      | Value of tab index                                    |
