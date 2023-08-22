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

| Props              | Type             | Required | Values | Default                   | Description                                |
| ------------------ | ---------------- | :------: | ------ | ------------------------- | ------------------------------------------ |
| `selectDateText`   | `string`         |    -     | -      | -                         | Select date text                           |
| `className`        | `string`         |    -     | -      | -                         | Accepts class                              |
| `initialDate`      | `date`, `string` |    -     | -      | -                         | Default date                               |
| `id`               | `string`         |    -     | -      | -                         | Accepts id                                 |
| `locale`           | `string`         |    -     | -      | `User's browser settings` | Browser locale                             |
| `maxDate`          | `date`, `string` |    -     | -      | -                         | Maximum date that the user can select.     |
| `minDate`          | `date`, `string` |    -     | -      | -                         | Minimum date that the user can select.     |
| `onChange`         | `func`           |    -     | -      | -                         | Function called when the user select a day |
| `showCalendarIcon` | `bool`           |    -     | -      | -                         | Shows calendar icon in selected item       |
| `outerDate`        | `obj`            |    -     | -      | -                         | Allows to track date outside the component |
| `openDate`         | `date`, `string` |    -     | -      | -                         | Allows to set first shown date in calendar |
