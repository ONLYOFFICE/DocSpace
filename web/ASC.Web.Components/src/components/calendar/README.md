# Calendar

#### Description

Custom calendar

#### Usage

```js
import { Calendar } from "asc-web-components";

<Calendar
  onChange={date => {
    console.log("Selected date:", date);
  }}
  disabled={false}
  themeColor="#ED7309"
  selectedDate={new Date()}
  openToDate={new Date()}
  minDate={new Date("1970/01/01")}
  maxDate={new Date("3000/01/01")}
  locale="ru"
/>;
```

#### Properties

| Props          | Type     | Required | Values | Default                  | Description                                                  |
| -------------- | -------- | :------: | ------ | ------------------------ | ------------------------------------------------------------ |
| `onChange`     | `func`   |    -     | -      | -                        | Function called when the user select a day                   |
| `isDisabled`   | `bool`   |    -     | -      | -                        | Disabled react-calendar                                      |
| `themeColor`   | `string` |    -     | -      | `#ED7309`                | Color of the selected day                                    |
| `selectedDate` | `date`   |    -     | -      | (today)                  | Selected date value                                          |
| `openToDate`   | `date`   |    -     | -      | (today)                  | The beginning of a period that shall be displayed by default |
| `minDate`      | `date`   |    -     | -      | `new Date("1970/01/01")` | Minimum date that the user can select.                       |
| `maxDate`      | `date`   |    -     | -      | `new Date("3000/01/01")` | Maximum date that the user can select.                       |
| `locale`       | `string` |    -     | -      | User's browser settings  | Browser locale                                               |

