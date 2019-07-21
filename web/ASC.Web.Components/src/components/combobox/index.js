import React from 'react'
import PropTypes from 'prop-types'
import styled from 'styled-components';
import InputBlock from '../input-block'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'

const StyledComboBox = styled.div`
    & > div > input {
        ${props => !props.isDisabled && `cursor: pointer !important;`}

        &::placeholder {
            font-family: Open Sans;
            font-style: normal;
            font-weight: 600;
            font-size: 13px;
            line-height: 20px;
            ${props => !props.isDisabled && `color: #333333;`}
            opacity: 1;
        }
    }
`;

class ComboBox extends React.Component {

    constructor(props) {
        super(props);

        this.ref = React.createRef();
    
        this.state = {
            isOpen: props.opened,
            boxLabel: props.items[0].label,
            items: props.items
        };
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

    render () {
        return (
            <StyledComboBox ref={this.ref} {...this.props}
                onClick={
                    !this.props.isDisabled
                    ? () => { 
                        this.setState({ items: this.props.items});
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
                    {this.state.items.map(item => 
                        <DropDownItem {...item}
                            onClick={() => { 
                                item.onClick && item.onClick();
                                this.toggle(!this.state.isOpen);
                                this.setState({boxLabel: item.label});
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
    isDisabled: PropTypes.bool
}

ComboBox.defaultProps = {
    isDisabled: false
}
  
export default ComboBox;