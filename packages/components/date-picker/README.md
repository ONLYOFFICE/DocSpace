# DatePicker

Base DatePicker component

### Usage

```js
import DatePicker from "@docspace/components/date-picker";
```

```jsx
<DatePicker
  onChange={(date) => {
    console.log("Selected date", date);
  }}
  date={new Date()}
  minDate={new Date("1970/01/01")}
  maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
  locale="en"
/>
```

#### Properties

| Props            | Type     | Required | Values | Default                   | Description                                |
| ---------------- | -------- | :------: | ------ | ------------------------- | ------------------------------------------ |
| `selectDateText` | `string` |    -     | -      | -                         | Select date text                           |
| `className`      | `string` |    -     | -      | -                         | Accepts class                              |
| `date`           | `date`   |    -     | -      | -                         | Calendar date                              |
| `id`             | `string` |    -     | -      | -                         | Accepts id                                 |
| `locale`         | `string` |    -     | -      | `User's browser settings` | Browser locale                             |
| `maxDate`        | `date`   |    -     | -      | -                         | Maximum date that the user can select.     |
| `minDate`        | `date`   |    -     | -      | -                         | Minimum date that the user can select.     |
| `onChange`       | `func`   |    -     | -      | -                         | Function called when the user select a day |
