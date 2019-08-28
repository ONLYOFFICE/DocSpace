import React from 'react';
import isEqual from 'lodash/isEqual';
import ComboBox from '../combobox'
import IconButton from '../icon-button';
import styled from 'styled-components';

const StyledIconButton = styled.div`
    transform: ${state => state.sortDirection ? 'scale(1, -1)' : 'scale(1)'};
`;
const StyledComboBox = styled(ComboBox)`
  display: block;
  float: left;
  width: 20%;
  margin-left: 8px;
`;
class SortComboBox extends React.Component {
    constructor(props) {
        super(props);
        this.onSelect = this.onSelect.bind(this);
    }
    onSelect(item) {
        this.props.onSelect(item);
    }
    shouldComponentUpdate(nextProps, nextState) {
        return !isEqual(this.props, nextProps);
    }
    render() {
        return (
            <StyledComboBox
                options={this.props.options}
                isDisabled={this.props.isDisabled}
                onSelect={this.onSelect}
                selectedOption={this.props.selectedOption}
            >
                <StyledIconButton sortDirection={this.props.sortDirection}>
                    <IconButton
                        color={"#D8D8D8"}
                        hoverColor={"#333"}
                        clickColor={"#333"}
                        size={10}
                        iconName={'ZASortingIcon'}
                        isFill={true}
                        isDisabled={this.props.isDisabled}
                        onClick={this.props.onButtonClick}
                    />
                </StyledIconButton>
            </StyledComboBox>
        );
    }
}

export default SortComboBox;