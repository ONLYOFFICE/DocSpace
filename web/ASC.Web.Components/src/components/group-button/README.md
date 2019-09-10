# GroupButton

## Usage

```js
import { GroupButton } from "asc-web-components";
```

#### Description

Base Button is used for a group action on a page.

It can be used as selector with checkbox for this following properties are combined: *isDropdown*, *isSeparator*.

*isSeparator* will add vertical bar after button.

*isDropdown* allows adding items to dropdown list in children.

For health of checkbox, button inherits part of properties of this component.

#### Usage

```js
<GroupButton
  label="Group button"
  disabled={false}
  isDropdown={false}
  opened={false}
/>
```

#### Properties

| Props               | Type     | Required | Values | Default        | Description                                           |
| ------------------- | -------- | :------: | ------ | -------------- | ----------------------------------------------------- |
| `label`             | `string` |    -     | -      | `Group button` | Value of the group button                             |
| `disabled`          | `bool`   |    -     | -      | `false`        | Tells when the button should present a disabled state |
| `isDropdown`        | `bool`   |    -     | -      | `false`        | Tells when the button should present a dropdown state |
| `isSeparator`       | `bool`   |    -     | -      | `false`        | Tells when the button should contain separator        |
| `opened`            | `bool`   |    -     | -      | `false`        | Tells when the button should be opened by default     |
| `action`            | `func`   |    -     | -      | -              | What the button will trigger when clicked             |
| `tabIndex`          | `number` |    -     | -      | `-1`           | Value of tab index                                    |
| `onClick`           | `func`   |    -     | -      | -              | Property for onClick action                           |
| `fontWeight`        | `string` |    -     | -      | `600`          | Value of font weight                                  |
| `onSelect`          | `func`   |    -     | -      | -              | Called when value is selected in selector             |
| `selected`          | `string` |    -     | -      | -              | Selected value label                                  |
| `onChange`          | `func`   |    -     | -      | -              | Called when checkbox is checked                       |
| `isIndeterminate`   | `bool`   |    -     | -      | `false`        | Initial value of Indeterminate checkbox               |
| `checked`           | `bool`   |    -     | -      | `false`        | Initial value of checkbox                             |
| `dropDownMaxHeight` | `number` |    -     | -      | -              | Selected height value of DropDown                     |
