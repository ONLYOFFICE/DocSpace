# SelectedItem

## Usage

```js
import { SelectedItem } from 'asc-web-components';
```


#### Usage

```js

<SelectedItem text="sample text" onClick={()=>console.log("onClose")}></SelectedItem>

```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `isDisabled`       | `bool`     |    -     | -                     | false     | Tells when the button should present a disabled state                   |
| `text`             | `string`   |    -     | -                     | -         | Selected item text                                                      |
| `isInline`         | `bool`     |    -     | -                     | true      | Sets the 'display: inline-block' property                               |
| `onClose`      | `function` |    -     | -                     | -         | What the selected item will trigger when clicked                        |



