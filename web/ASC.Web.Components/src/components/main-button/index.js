import React from 'react';
import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';
import DropDown from '../drop-down';
import { Icons } from '../icons';

const backgroundColor = '#ED7309',
    disableBackgroundColor = '#FFCCA6',
    hoverBackgroundColor = '#FF8932',
    clickBackgroundColor = '#C96C27';

const hoveredCss = css` 
    background-color: ${hoverBackgroundColor};
    cursor: pointer;
`;
const clickCss = css`
    background-color: ${clickBackgroundColor};
    cursor: pointer;
`;

const arrowDropdown = css`
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    border-top: 4px solid white;
    content: "";
    height: 0;
    margin-top: -1px;
    position: absolute;
    right: 8px;
    top: 50%;
    width: 0;
`;

const notDisableStyles = css`
    &:hover {
        ${hoveredCss}
    };

    &:active {
        ${clickCss}
    };
`;

const notDropdown = css`
    &:after {
        display:none;
    }

    border-top-right-radius: 0;
    border-bottom-right-radius:0;
    
`;

const GroupMainButton = styled.div`
    position: relative;
    display: grid;
    Grid-template-columns: ${props => (props.isDropdown ? "1fr" : "1fr 32px")};
    ${props => (!props.isDropdown && "grid-column-gap: 1px")};
`;

const StyledDropDown = styled(DropDown)`
    width:100%;
`;

const StyledMainButton = styled.div`
    position: relative;
    display: block;
    vertical-align: middle;
    box-sizing: border-box;
    background-color: ${props => props.isDisabled ? disableBackgroundColor : backgroundColor};
    padding: 5px 11px;
    color: #ffffff;
    border-radius: 3px;
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    font-weight: bold;
    font-size: 16px;
    line-height: 22px;

    &:after {
        ${arrowDropdown}
    }

    ${props => (!props.isDisabled && notDisableStyles)}
    ${props => (!props.isDropdown && notDropdown)}

    & > svg {
        display: block;
        margin: auto;
        height: 100%;
    } 
`;

const StyledSecondaryButton = styled(StyledMainButton)`
    display: inline-block;
    height: 32px;
    padding:0;
    border-radius: 3px;
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    border-top-left-radius: 0;
    border-bottom-left-radius:0;
`;

class MainButton extends React.PureComponent {
    constructor(props) {
        super(props);

        this.ref = React.createRef();
        this.iconNames = Object.keys(Icons);

        this.state = {
            isOpen: props.opened
        };

        this.handleClick.bind(this);
        this.stopAction.bind(this);
        this.toggle.bind(this);
    }

    handleClick = (e) => !this.ref.current.contains(e.target) && this.toggle(false);
    stopAction = (e) => e.preventDefault();
    toggle = (isOpen) => this.setState({ isOpen: isOpen});

    componentDidMount() {
        if (this.ref.current) {
            document.addEventListener("click", this.handleClick);
        }
    }

    componentWillUnmount() {
        document.removeEventListener("click", this.handleClick)
    }

    componentDidUpdate(prevProps) {
        // Store prevId in state so we can compare when props change.
        // Clear out previously-loaded data (so we don't render stale stuff).
        if (this.props.opened !== prevProps.opened) {
          this.toggle(this.props.opened);
        }
      }

    render() {
        //console.log("MainButton render");
        return (
            <GroupMainButton {...this.props} ref={this.ref}>
                <StyledMainButton {...this.props} onClick={
                    !this.props.isDisabled
                        ? !this.props.isDropdown
                            ? this.props.clickAction
                            : () => { this.toggle(!this.state.isOpen) }
                        : this.stopAction}>
                    {this.props.text}
                </StyledMainButton>
                {this.props.isDropdown
                    ? <StyledDropDown isOpen={this.state.isOpen} {...this.props} onClick={() => {
                        this.props.onClick && this.props.onClick();
                        this.toggle(!this.state.isOpen);
                    }} />
                    : <StyledSecondaryButton {...this.props} onClick={!this.props.isDisabled ? this.props.clickActionSecondary : this.stopAction}>
                        {
                            this.iconNames.includes(this.props.iconName)
                            && React.createElement(Icons[this.props.iconName], { size: "medium", color: "#ffffff" })
                        }
                    </StyledSecondaryButton>}
            </GroupMainButton>
        )
    };
}

MainButton.propTypes = {
    text: PropTypes.string,
    isDisabled: PropTypes.bool,
    isDropdown: PropTypes.bool,
    clickAction: PropTypes.func,
    clickActionSecondary: PropTypes.func,
    iconName: PropTypes.string,
};

MainButton.defaultProps = {
    text: "Button",
    isDisabled: false,
    isDropdown: true,
    iconName: "PeopleIcon",
};

export default MainButton;