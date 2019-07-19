import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import SearchInput from '../search-input';
import ComboBox from '../combobox'

class FilterInput extends React.Component {
    constructor(props) {
        super(props);
    
        this.state = {};
    }

    render(){
        return(
            <div>
                <SearchInput 
                    id={this.props.id}
                    isDisabled={this.props.isDisabled}
                    size={this.props.size}
                    scale={this.props.scale}
                    isNeedFilter={true}
                    getFilterData={this.props.getFilterData}
                    placeholder={this.props.placeholder}
                    value={value}
                />
                <ComboBox 
                    items={this.props.getFilterData}
                    isDisabled={this.props.isDisabled}
                />
            </div>
            
        );
    }
}

export default FilterInput;