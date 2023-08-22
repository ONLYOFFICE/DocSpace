# TimePicker

Time input

### Usage

```js
import TimePicker from "@docspace/components/time-picker";
```

```jsx
<TimePicker
  date={new Date()}
  setDate={setDate}
  hasError={true}
  onChange={(date) => console.log(date)}
/>
```

#### Properties

| Props           |   Type   | Required | Values | Default | Description                                      |
| --------------- | :------: | :------: | :----: | :-----: | ------------------------------------------------ |
| `className`     | `string` |    -     |   -    |   ''    | Allows to set classname                          |
| `initialDate`   | `object` |    -     |   -    |    -    | Inital date                                      |
| `onChange`      |  `func`  |    -     |   -    |    -    | Allow you to handle changing events of component |
| `hasError`      |  `bool`  |    -     |   -    |  false  | Indicates error                                  |
| `onBlur`        |  `func`  |    -     |   -    |    -    | Triggers function on blur                        |
| `focusOnRender` |  `bool`  |    -     |   -    |  false  | Focus input on render                            |
| `forwardedRef`  | `object` |    -     |   -    |  false  | Passes ref to child component                    |
