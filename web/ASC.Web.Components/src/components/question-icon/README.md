# ToggleButton

#### Description

Custom toggle button input

#### Usage

```js
import { QuestionIcon } from "asc-web-components";

const dropDownBody = (
  <Text.Body style={{ padding: "16px" }}>
    {`Время существования сессии по умолчанию составляет 20 минут.
    Отметьте эту опцию, чтобы установить значение 1 год.
    Чтобы задать собственное значение, перейдите в настройки.`}
  </Text.Body>
);

<QuestionIcon
  dropDownBody={dropDownBody}
  dropDownDirectionY="bottom"
  dropDownDirectionX="left"
  dropDownManualY={0}
  dropDownManualX={0}
  dropDownManualWidth={300}
  backgroundColor="#fff"
  isOpen={false}
  size={12}
  onClick={() => {
    console.log("QuestionIcon clicked");
  }}
/>;
```

#### Properties

| Props                 | Type     | Required | Values          | Default | Description                                                                        |
| --------------------- | -------- | :------: | --------------- | ------- | ---------------------------------------------------------------------------------- |
| `dropDownBody`        | `object` |    -     | -               | -       | Drop-down body                                                                     |
| `dropDownDirectionY`  | `number` |    -     | `top`, `bottom` | `top`   | Sets the opening direction relative to the parent                                  |
| `dropDownDirectionX`  | `number` |    -     | `left`, `right` | `left`  | Sets the opening direction relative to the parent                                  |
| `dropDownManualWidth` | `number` |    -     | -               | -       | Required if you need to specify the exact width of the component, for example 100% |
| `dropDownManualY`     | `number` |    -     | -               | -       | Required if you need to specify the exact distance from the parent component       |
| `dropDownManualX`     | `number` |    -     | -               | -       | Required if you need to specify the exact distance from the parent component       |
| `backgroundColor`     | `string` |    -     | -               | `#fff`  | Background icon color                                                              |
| `isOpen`              | `bool`   |    -     | -               | `false` | Tells when the dropdown should be opened                                           |
| `size`                | `number` |    -     | -               | -       | Set question icon size                                                             |
| `onClick`             | `func`   |    -     | -               | -       | Will be triggered whenever an QuestionIcon is clicked                              |
