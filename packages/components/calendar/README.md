# Calendar

Used to display custom calendar

### Usage

```js
import Calendar from "@docspace/components/calendar";
```

```jsx
<Calendar
  onChange={(date) => {
    console.log("Selected date:", date);
  }}
  minDate={new Date("1970/01/01")}
  maxDate={new Date("3000/01/01")}
  locale="ru"
/>
```

#### Properties

| Props             |       Type       | Required | Values |          Default          | Description                                |
| ----------------- | :--------------: | :------: | :----: | :-----------------------: | ------------------------------------------ |
| `className`       |     `string`     |    -     |   -    |             -             | Accepts class                              |
| `id`              |     `string`     |    -     |   -    |             -             | Accepts id                                 |
| `locale`          |     `string`     |    -     |   -    | `User's browser settings` | Browser locale                             |
| `maxDate`         | `date`, `string` |    -     |   -    | `new Date("3000/01/01")`  | Maximum date that the user can select.     |
| `minDate`         | `date`, `string` |    -     |   -    | `new Date("1970/01/01")`  | Minimum date that the user can select.     |
| `onChange`        |      `func`      |    -     |   -    |             -             | Function called when the user select a day |
| `style   `        |   `obj`, `arr`   |    -     |   -    |             -             | Accepts css style                          |
| `initialDate`     |      `date`      |    -     |   -    |       `new Date()`        | First shown date.                          |
| `selectedDate`    |      `date`      |    -     |   -    |             -             | Selected date                              |
| `setSelectedDate` |      `date`      |    -     |   -    |             -             | Setter for selected date                   |
