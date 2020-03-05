import React from 'react';
import isEqual from 'lodash/isEqual';
import { ComboBox, IconButton, DropDownItem, RadioButtonGroup } from 'asc-web-components';
import PropTypes from 'prop-types';
import { StyledIconButton } from '../StyledFilterInput';

class SortComboBox extends React.Component {
    constructor(props) {
        super(props);

        const { sortDirection } = props;

        this.state = {
            sortDirection
        }

        this.combobox = React.createRef();
    }
    onButtonClick = () => {
        const { onChangeSortDirection } = this.props;
        const { sortDirection } = this.state;
        typeof onChangeSortDirection === 'function' && onChangeSortDirection(+(sortDirection === 0 ? 1 : 0));
        this.setState({
            sortDirection: sortDirection === 0 ? 1 : 0
        });
    }

    onChangeSortId = (e) => {
        const { onChangeSortId } = this.props;
        typeof onChangeSortId === 'function' && onChangeSortId(e.target.value);
    }
    onChangeSortDirection = (e) => {
        const sortDirection = +e.target.value;
        const { onChangeSortDirection } = this.props;
        this.setState({ sortDirection });
        typeof onChangeSortDirection === 'function' && onChangeSortDirection(sortDirection);
    }
    shouldComponentUpdate(nextProps, nextState) {
        //TODO 
        /*const comboboxText = this.combobox.current.ref.current.children[0].children[1];
        if(comboboxText.scrollWidth > Math.round(comboboxText.getBoundingClientRect().width)){
            comboboxText.style.opacity = "0";
        }else{
            comboboxText.style.opacity = "1";
        }*/
        const { sortDirection } = nextProps;
        if (this.props.sortDirection !== sortDirection) {
            this.setState({
                sortDirection
            });
            return true;
        }
        return (!isEqual(this.props, nextProps) || !isEqual(this.state, nextState));
    }
    render() {
        const { options, directionAscLabel, directionDescLabel, isDisabled,
            selectedOption } = this.props;
            const { sortDirection } = this.state;
        let sortArray = options.map(function (item) {
            item.value = item.key
            return item;
        });
        let sortDirectionArray = [
            { value: '0', label: directionAscLabel },
            { value: '1', label: directionDescLabel }
        ];

        const isMobile = window.innerWidth > 375; //TODO: Make some better

        const advancedOptions = (
            <>
                <DropDownItem noHover >
                    <RadioButtonGroup
                        fontWeight={600}
                        isDisabled={isDisabled}
                        name={'direction'}
                        onClick={this.onChangeSortDirection}
                        options={sortDirectionArray}
                        orientation='vertical'
                        selected={sortDirection.toString()}
                        spacing='0px'
                    />
                </DropDownItem>
                <DropDownItem isSeparator />
                <DropDownItem noHover >
                    <RadioButtonGroup
                        fontWeight={600}
                        isDisabled={isDisabled}
                        name={'sort'}
                        onClick={this.onChangeSortId}
                        options={sortArray}
                        orientation='vertical'
                        selected={selectedOption.key}
                        spacing='0px'
                    />
                </DropDownItem>
            </>
        );
        return (
            <ComboBox
                advancedOptions={advancedOptions}
                className='styled-sort-combobox'
                directionX="right"
                isDisabled={isDisabled}
                options={[]}
                ref={this.combobox}
                scaled={true}
                scaledOptions={isMobile}
                selectedOption={selectedOption}
                size="content"
            >
                <StyledIconButton sortDirection={!!sortDirection}>
                    <IconButton
                        clickColor={"#333"}
                        color={"#A3A9AE"}
                        hoverColor={"#333"}
                        iconName={'ZASortingIcon'}
                        isDisabled={isDisabled}
                        isFill={true}
                        onClick={this.onButtonClick}
                        size={10}
                    />
                </StyledIconButton>
            </ComboBox>
        );
    }
}

SortComboBox.propTypes = {
    directionAscLabel: PropTypes.string,
    directionDescLabel: PropTypes.string,
    isDisabled: PropTypes.bool,
    onButtonClick: PropTypes.func,
    onChangeSortDirection: PropTypes.func,
    onChangeSortId: PropTypes.func,
    sortDirection: PropTypes.number,
}

SortComboBox.defaultProps = {
    isDisabled: false,
    sortDirection: 0
}


export default SortComboBox;