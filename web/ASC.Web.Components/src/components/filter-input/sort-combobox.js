import React from 'react';
import isEqual from 'lodash/isEqual';
import ComboBox from '../combobox'
import IconButton from '../icon-button';
import DropDownItem from '../drop-down-item';
import RadioButtonGroup from '../radio-button-group'
import styled from 'styled-components';
import PropTypes from 'prop-types';
import { mobile } from '../../utils/device'

const StyledIconButton = styled.div`
    transform: ${state => !state.sortDirection ? 'scale(1, -1)' : 'scale(1)'};
`;
const StyledComboBox = styled(ComboBox)`
    display: block;
    float: left;
    width: 132px;
    margin-left: 8px;

    @media ${mobile} {
        width: 50px;
        .optionalBlock ~ div:first-child{
            opacity: 0
        }
    }

    .combo-button-label {
        color: #A3A9AE;
    }

`;

class SortComboBox extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            sortDirection: this.props.sortDirection
        }

        this.combobox = React.createRef();

        this.onChangeSortId = this.onChangeSortId.bind(this);
        this.onChangeSortDirection = this.onChangeSortDirection.bind(this);
        this.onButtonClick = this.onButtonClick.bind(this);

    }
    onButtonClick() {
        typeof this.props.onChangeSortDirection === 'function' && this.props.onChangeSortDirection(+(this.state.sortDirection === 0 ? 1 : 0));
        this.setState({
            sortDirection: this.state.sortDirection === 0 ? 1 : 0
        });
    }

    onChangeSortId(e) {
        typeof this.props.onChangeSortId === 'function' && this.props.onChangeSortId(e.target.value);
    }
    onChangeSortDirection(e) {
        this.setState({
            sortDirection: +e.target.value
        });
        typeof this.props.onChangeSortDirection === 'function' && this.props.onChangeSortDirection(+e.target.value);
    }
    shouldComponentUpdate(nextProps, nextState) {
        //TODO 
        /*const comboboxText = this.combobox.current.ref.current.children[0].children[1];
        if(comboboxText.scrollWidth > Math.round(comboboxText.getBoundingClientRect().width)){
            comboboxText.style.opacity = "0";
        }else{
            comboboxText.style.opacity = "1";
        }*/
        if (this.props.sortDirection !== nextProps.sortDirection) {
            this.setState({
                sortDirection: nextProps.sortDirection
            });
            return true;
        }
        return (!isEqual(this.props, nextProps) || !isEqual(this.state, nextState));
    }
    render() {
        let sortArray = this.props.options.map(function (item) {
            item.value = item.key
            return item;
        });
        let sortDirectionArray = [
            { value: '0', label: this.props.directionAscLabel },
            { value: '1', label: this.props.directionDescLabel }
        ];

        const isMobile = window.innerWidth > 375; //TODO: Make some better

        const advancedOptions = (
            <>
                <DropDownItem noHover >
                    <RadioButtonGroup
                        orientation='vertical'
                        onClick={this.onChangeSortDirection}
                        isDisabled={this.props.isDisabled}
                        selected={this.state.sortDirection.toString()}
                        spacing='0px'
                        name={'direction'}
                        options={sortDirectionArray}
                        fontWeight ={600}
                    />
                </DropDownItem>
                <DropDownItem isSeparator />
                <DropDownItem noHover >
                    <RadioButtonGroup
                        orientation='vertical'
                        onClick={this.onChangeSortId}
                        isDisabled={this.props.isDisabled}
                        selected={this.props.selectedOption.key}
                        spacing='0px'
                        name={'sort'}
                        options={sortArray}
                        fontWeight ={600}
                    />
                </DropDownItem>
            </>
        );
        return (
            <StyledComboBox
                ref={this.combobox}
                options={[]}
                advancedOptions={advancedOptions}
                isDisabled={this.props.isDisabled}
                selectedOption={this.props.selectedOption}
                scaled={true}
                scaledOptions={isMobile}
                size="content"
                directionX="right"
            >
                <StyledIconButton sortDirection={!!this.state.sortDirection}>
                    <IconButton
                        color={"#A3A9AE"}
                        hoverColor={"#333"}
                        clickColor={"#333"}
                        size={10}
                        iconName={'ZASortingIcon'}
                        isFill={true}
                        isDisabled={this.props.isDisabled}
                        onClick={this.onButtonClick}
                    />
                </StyledIconButton>
            </StyledComboBox>
        );
    }
}

SortComboBox.propTypes = {
    isDisabled: PropTypes.bool,
    sortDirection: PropTypes.number,
    onChangeSortId: PropTypes.func,
    onChangeSortDirection: PropTypes.func,
    onButtonClick: PropTypes.func,
    directionAscLabel: PropTypes.string,
    directionDescLabel: PropTypes.string
}

SortComboBox.defaultProps = {
    isDisabled: false,
    sortDirection: 0
}


export default SortComboBox;