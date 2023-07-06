# DateTimePicker

Date-time input

### Usage

```js
import DateTimePicker from "@docspace/components/date-time-picker";
```

```jsx
<DateTimePicker
  onChange={onChange}
  date={moment("2022-10-20", "YYYY-MM-DD")}
  selectDateText={t("SelectDate")}
  className="datePicker"
  id="datePicker"
/>
```

#### Properties

| Props            |   Type   | Required | Values |    Default    | Description                                      |
| ---------------- | :------: | :------: | :----: | :-----------: | ------------------------------------------------ |
| `className`      | `string` |    -     |   -    |      ''       | Allows to set classname                          |
| `id`             | `string` |    -     |   -    |       -       | Allows to set id                                 |
| `onChange`       |  `func`  |    -     |   -    |       -       | Allow you to handle changing events of component |
| `date`           |  `obj`   |    -     |   -    |       -       | Date object                                      |
| `selectDateText` | `string` |    -     |   -    | "Select date" | Select date text                                 |
| `setDate`        |  `func`  |    -     |   -    |       -       | Sets date                                        |
