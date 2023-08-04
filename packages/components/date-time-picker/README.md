# DateTimePicker

Date-time input

### Usage

```js
import DateTimePicker from "@docspace/components/date-time-picker";
```

```jsx
<DateTimePicker
  onChange={onChange}
  selectDateText={t("SelectDate")}
  className="datePicker"
  id="datePicker"
/>
```

#### Properties

| Props            |       Type       | Required | Values |          Default          | Description                                      |
| ---------------- | :--------------: | :------: | :----: | :-----------------------: | ------------------------------------------------ |
| `className`      |     `string`     |    -     |   -    |            ''             | Allows to set classname                          |
| `id`             |     `string`     |    -     |   -    |             -             | Allows to set id                                 |
| `onChange`       |      `func`      |    -     |   -    |             -             | Allow you to handle changing events of component |
| `initialDate`    | `date`, `string` |    -     |   -    |             -             | Default date                                     |
| `selectDateText` |     `string`     |    -     |   -    |       "Select date"       | Select date text                                 |
| `locale`         |     `string`     |    -     |   -    | `User's browser settings` | Browser locale                                   |
| `maxDate`        | `date`, `string` |    -     |   -    |             -             | Maximum date that the user can select.           |
| `minDate`        | `date`, `string` |    -     |   -    |             -             | Minimum date that the user can select.           |
| `hasError`       |    `boolean`     |    -     |   -    |             -             | Minimum date that the user can select.           |
| `openDate`       | `date`, `string` |    -     |   -    |             -             | Allows to set first shown date in calendar       |
