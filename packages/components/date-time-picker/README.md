# DateTimePicker

Date-time input

### Usage

```js
import DateTimePicker from "@docspace/components/date-time-picker";
```

```jsx
<DateTimePicker
  isApplied={isApplied}
  setIsApplied={setIsApplied}
  onChange={onChange}
  initialDate={moment("2022-10-20", "YYYY-MM-DD")}
  initialTimeFrom={moment("12:10", "HH:mm")}
  initialTimeTo={moment("22:10", "HH:mm")}
  selectDateText={t("SelectDate")}
  fromText={t("From")}
  beforeText={t("Before")}
  selectTimeText={t("SelectDeliveryTime")}
  className="datePicker"
  isLimit={false}
/>
```

#### Properties

| Props             |   Type   | Required | Values |    Default    | Description                                      |
| ----------------- | :------: | :------: | :----: | :-----------: | ------------------------------------------------ |
| `className`       | `string` |    -     |   -    |      ''       | Allows to set classname                          |
| `id`              | `string` |    -     |   -    |       -       | Allows to set id                                 |
| `onChange`        |  `func`  |    -     |   -    |       -       | Allow you to handle changing events of component |
| `initialDate`     |  `obj`   |    -     |   -    |     false     | Inital date                                      |
| `initialTimeFrom` |  `obj`   |    -     |   -    |     false     | Inital time from                                 |
| `initialTimeTo`   |  `obj`   |    -     |   -    |     false     | Inital time to                                   |
| `fromText`        | `string` |    -     |   -    |    "From"     | From text                                        |
| `beforeText`      | `string` |    -     |   -    |   "Before"    | Before text                                      |
| `selectDateText`  | `string` |    -     |   -    | "Select date" | Select date text                                 |
| `selectTimeText`  | `string` |    -     |   -    | "Select time" | Select time text                                 |
| `isApplied`       |  `bool`  |    -     |   -    |       -       | Combines the limit into one date block           |
| `setIsApplied`    |  `func`  |    -     |   -    |       -       | Allows to set isApplied status                   |
| `isLimit`         |  `bool`  |    -     |   -    |     false     | Hides before selector                            |
