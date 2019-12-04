# DropDown

Is a dropdown with any number of action

### Usage

```js
import { DropDown } from "asc-web-components";
```

```jsx
<DropDown opened={false}></DropDown>
```

By default, it is used with DropDownItem elements in role of children.

If you want to display something custom, you can put it in children, but take into account that all stylization is assigned to the implemented component.

When using component, it should be noted that parent must have CSS property _position: relative_. Otherwise, DropDown will appear outside parent's border.

### Properties

| Props         |   Type   | Required |     Values      | Default  | Description                                                                            |
| ------------- | :------: | :------: | :-------------: | :------: | -------------------------------------------------------------------------------------- |
| `opened`      |  `bool`  |    -     |        -        | `false`  | Tells when the dropdown should be opened                                               |
| `directionX`  | `oneOf`  |    -     | `left`, `right` |  `left`  | Sets the opening direction relative to the parent                                      |
| `directionY`  | `oneOf`  |    -     | `top`, `bottom` | `bottom` | Sets the opening direction relative to the parent                                      |
| `manualWidth` | `string` |    -     |        -        |    -     | Required if you need to specify the exact width of the component, for example 100%     |
| `manualY`     | `string` |    -     |        -        |    -     | Required if you need to specify the exact distance from the parent component           |
| `manualX`     | `string` |    -     |        -        |    -     | Required if you need to specify the exact distance from the parent component           |
| `maxHeight`   | `number` |    -     |        -        |    -     | Required if the scrollbar is displayed                                                 |
| `withArrow`   |  `bool`  |    -     |        -        | `false`  | It is used if it is necessary to display blue protruding angle as when viewing profile |
