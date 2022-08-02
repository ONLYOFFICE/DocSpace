# DropDown

Is a dropdown with any number of action

### Usage

```js
import DropDown from "@docspace/components/drop-down";
```

```jsx
<DropDown opened={false}></DropDown>
```

By default, it is used with DropDownItem elements in role of children.

If you want to display something custom, you can put it in children, but take into account that all stylization is assigned to the implemented component.

When using component, it should be noted that parent must have CSS property _position: relative_. Otherwise, DropDown will appear outside parent's border.

### Properties

| Props                |      Type      | Required |     Values      | Default  | Description                                                                        |
| -------------------- | :------------: | :------: | :-------------: | :------: | ---------------------------------------------------------------------------------- |
| `className`          |    `string`    |    -     |        -        |    -     | Accepts class                                                                      |
| `clickOutsideAction` |     `func`     |    -     |        -        |    -     | Required for determining a click outside DropDown with the withBackdrop parameter  |
| `directionX`         |    `oneOf`     |    -     | `left`, `right` |  `left`  | Sets the opening direction relative to the parent                                  |
| `directionY`         |    `oneOf`     |    -     | `top`, `bottom` | `bottom` | Sets the opening direction relative to the parent                                  |
| `id`                 |    `string`    |    -     |        -        |    -     | Accepts id                                                                         |
| `manualWidth`        |    `string`    |    -     |        -        |    -     | Required if you need to specify the exact width of the component, for example 100% |
| `manualX`            |    `string`    |    -     |        -        |    -     | Required if you need to specify the exact distance from the parent component       |
| `manualY`            |    `string`    |    -     |        -        |    -     | Required if you need to specify the exact distance from the parent component       |
| `maxHeight`          |    `number`    |    -     |        -        |    -     | Required if the scrollbar is displayed                                             |
| `open`               |     `bool`     |    -     |        -        | `false`  | Tells when the dropdown should be opened                                           |
| `style`              | `obj`, `array` |    -     |        -        |    -     | Accepts css style                                                                  |
| `withBackdrop`       |     `bool`     |    -     |        -        |  `true`  | Used to display backdrop                                                           |
| `showDisabledItems`  |     `bool`     |    -     |        -        | `false`  | Display disabled items or not                                                      |
| `withBlur`           |     `bool`     |    -     |        -        | `false`  | Enable blur for backdrop                                                           |
