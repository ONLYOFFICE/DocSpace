# Input: FilterInput

## Usage

```js
import { FilterInput } from 'asc-web-components';
```

#### Description

FilterInput description

#### Usage

```js
<FilterInput 
    getFilterData={() => [  { key: 'filter-example', group: 'filter-example', label: 'example group', isHeader: true },
                            { key: 'filter-example-test', group: 'filter-example', label: 'Test' }]
                    }
    getSortData={() => [  { id: 'name', label: 'Name' },
                          { id: 'surname', label: 'Surname']
    onFilter={(result) => {console.log(result)}}
/>
```


| Props                  | Type     | Required | Values                         | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ----------------------------   | ------- | ------------------------------------------------------------------------------------------------------ |
| `id`                   | `string` |    -     | -                              | -       | Used as HTML `id` property                                                                             |
| `value`                | `string` |    -     | -                              | -       | Value of the input                                                                                     |
| `onChange`             | `func`   |    -     | -                              | -       | Called with the new value. Required when input is not read only. Parent should pass it back as `value` |
| `isDisabled`           | `bool`   |    -     | -                              | `false` | Indicates that the field cannot be used (e.g not authorised, or changes not saved)                     |
| `placeholder`          | `string` |    -     | -                              | -       | Placeholder text for the input                                                                         |
| `size`                 | `string` |          | `base`, `middle`, `big`, `huge`| `base`  | Supported size of the input fields.                                                                    |
| `scale`                | `bool`   |    -     | -                              | -       | Indicates the input field has scale                                                                    |


