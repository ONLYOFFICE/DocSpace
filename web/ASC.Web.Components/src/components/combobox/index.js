import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import InputBlock from '../input-block'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'

const StyledComboBox = styled.div`
    & > div {
        ${props => !props.withBorder && `border-color: transparent;`} 
    }
     
    & > div > input {
        ${props => !props.isDisabled && `cursor: pointer !important;`}
        ${props => !props.withBorder && `border-color: transparent !important;`}
        
        &::placeholder {
            font-family: Open Sans;
            font-style: normal;
            font-weight: 600;
            font-size: 13px;
            line-height: 20px;
            ${props => !props.isDisabled && `color: #333333;`}
            ${props => (!props.withBorder & !props.isDisabled) && `border-bottom: 1px dotted #333333;`}
            opacity: 1;
        }
    }
`;

class ComboBox extends React.PureComponent {
    constructor(props) {
        super(props);

        this.ref = React.createRef();

        this.state = {
            isOpen: props.opened,
            boxLabel: props.options[props.selectedIndex].label,
            options: props.options
        };

        this.handleClick = this.handleClick.bind(this);
        this.stopAction = this.stopAction.bind(this);
        this.toggle = this.toggle.bind(this);
    }

    handleClick = (e) => !this.ref.current.contains(e.target) && this.toggle(false);
    stopAction = (e) => e.preventDefault();
    toggle = (isOpen) => this.setState({ isOpen: isOpen });

    componentDidMount() {
        if (this.ref.current) {
            document.addEventListener("click", this.handleClick);
        }
    }

    componentWillUnmount() {
        document.removeEventListener("click", this.handleClick)
    }

    componentDidUpdate(prevProps) {
        if (this.props.opened !== prevProps.opened) {
            this.toggle(this.props.opened);
        }
    }

    render() {
        //console.log("ComboBox render");
        return (
            <StyledComboBox ref={this.ref} {...this.props} data={this.state.boxLabel}
                onClick={
                    !this.props.isDisabled
                        ? () => {
                            this.setState({ option: this.props.option });
                            this.toggle(!this.state.isOpen);
                        }
                        : this.stopAction
                }
            >
                <InputBlock placeholder={this.state.boxLabel}
                    iconName='ExpanderDownIcon'
                    iconSize={8}
                    isIconFill={true}
                    iconColor='#A3A9AE'
                    scale={true}
                    isDisabled={this.props.isDisabled}
                    isReadOnly={true}
                    onIconClick={() => false}
                >
                    {this.props.children}
                    <DropDown
                        directionX={this.props.directionX}
                        directionY={this.props.directionY}
                        manualWidth='100%'
                        manualY='102%'
                        isOpen={this.state.isOpen}
                    >
                        {this.state.options.map((option, index) =>
                            <DropDownItem {...option}
                                disabled={option.label === this.state.boxLabel}
                                onClick={(e) => {
                                    option.onClick && option.onClick(e);
                                    this.toggle(!this.state.isOpen);
                                    this.setState({ boxLabel: option.label });
                                }}
                            />
                        )}
                    </DropDown>
                </InputBlock>
            </StyledComboBox>
        );
    }
};

ComboBox.propTypes = {
    isDisabled: PropTypes.bool,
    withBorder: PropTypes.bool,
    selectedIndex: PropTypes.number,
    options: PropTypes.array
}

ComboBox.defaultProps = {
    isDisabled: false,
    withBorder: true,
    selectedIndex: 0
}

export default ComboBox;