# DatePicker

#### Description

DatePicker

#### Usage

```js
import { DatePicker } from 'asc-web-components';

<DatePicker
        onChange={date => {
          console.log('Selected date', date);
        }}
        selectedDate={new Date()}
        minDate={new Date("1970/01/01")}
        maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
      />
```

#### Properties


| Props        | Type     | Required | Values | Default | Description                             |
| ------------ | -------- | :------: | ------ | ------- | --------------------------------------- |
| `onChange`   | `func`   |    -     | -      | -       | Function called when the user select a day |
| `disabled`   | `bool`   |    -     | -      | -       | Disabled react-calendar |
| `themeColor` | `string` |    -     | -      | `#ED7309`       | Color of the selected day  |
| `selectedDate`   | `date`   |    -     | -      | (today)  | Selected date value  |
| `openToDate`   | `date`   |    -     | -      | (today)  | The beginning of a period that shall be displayed by default |
| `minDate`   | `date`   |    -     | -      | `new Date("1970/01/01")`  | Minimum date that the user can select.  |
| `maxDate`   | `date`   |    -     | -      | `new Date("3000/01/01")`  | Maximum date that the user can select.  |
| `locale`   | `string`   |    -     | -      | User's browser settings       | Browser locale  |
| `scaled` | `bool`   |    -     |       |  | Selected calendar size |