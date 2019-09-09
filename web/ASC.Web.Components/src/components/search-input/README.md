# Input: SearchInput

## Usage

```js
import { SearchInput } from 'asc-web-components';
```

#### Description

SearchInput description

#### Usage

```js
<SearchInput 
    isNeedFilter={true}
    getFilterData={() => [  { key: 'filter-example', group: 'filter-example', label: 'example group', isHeader: true },
                            { key: 'filter-example-test', group: 'filter-example', label: 'Test' }]
                    }
    onSearchClick={(result) => {console.log(result)}}
    onChangeFilter={(result) => {console.log(result)}}
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
| `isNeedFilter`         | `bool`   |          |                                | `false` | Determines if filter is needed                                                                         |
| `selectedFilterData`   | `array`  |    -     |                                |         | Selected filter data                                                                                   |

