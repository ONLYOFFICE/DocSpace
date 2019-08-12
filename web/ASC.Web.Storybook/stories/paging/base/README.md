# Paging

#### Description

Paging is used to navigate med content pages.

#### Usage

```js
import { Paging } from 'asc-web-components';

<ComboBox 
    previousLabel='Previous' 
    nextLabel='Next'
    pageItems={pageItems} 
    countItems={countItems} 
    previousAction={previousAction} 
    nextAction={nextAction} 
    openDirection='bottom'
    disablePrevious={false}
    disableNext={false}
    selectedPage={1}
    selectedCount={25}
    emptyPagePlaceholder='1 of 1'
    emptyCountPlaceholder='25 per page'
/>
```

#### Properties

| Props                  | Type              | Required | Values                       | Default | Description                                  |
| ---------------------- | ----------------- | :------: | ---------------------------- | ------- | -------------------------------------------- |
| `pageItems`            | `array`           |    -     | -                            | -       | Paging combo box items                       |
| `countItems`           | `array`           |    -     | -                            | -       | Items per page combo box items               |
| `previousLabel`        | `string`          |    -     | -                            | `Previous`| Label for previous button                  |
| `nextLabel`            | `string`          |    -     | -                            | `Next`  | Label for next button                        |
| `previousAction`       | `function`        |    -     | -                            | -       | Action for previous button                   |
| `nextAction`           | `function`        |    -     | -                            | -       | Action for next button                       |
| `openDirection`        | `string`          |    -     | `top`, `bottom`              | `bottom`| Indicates opening direction of combo box     |
| `disablePrevious`      | `bool`            |    -     | -                            | `false` | Set previous button disabled                 |
| `disableNext`          | `bool`            |    -     | -                            | `false` | Set next button disabled                     |
| `selectedPage`         | `string`,`number` |    -     | -                            | -       | Initial value for pageItems                  |
| `selectedCount`        | `string`,`number` |    -     | -                            | -       | Initial value for countItems                 |
| `emptyPagePlaceholder` | `string`          |    -     | -                            | -       | Value that will be displayed in page selection when collection is empty|
| `emptyCountPlaceholder`| `string`          |    -     | -                            | -       | Value that will be displayed in count selection when collection is empty|
