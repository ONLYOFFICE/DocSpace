import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Icons } from '../icons';
import DropDown from '../drop-down';
import DropDownItem from '../drop-down-item';
import { Text } from '../text';
import { handleAnyClick } from '../../utils/event';

const SimpleLinkWithDropdown = ({ isBold, fontSize, isTextOverflow,
    isHovered, isSemitransparent, color, title,
    dropdownType, data, ...props }) => <a {...props}></a>;

const getColor = color => {
    switch (color) {
        case 'gray':
            return '#A3A9AE';
        case 'blue':
            return '#316DAA';
        default:
            return '#333333';
    }
}

const opacityCss = css`
    ${props => (props.isSemitransparent && `opacity: 0.5`)};
`;

const colorCss = css`
    color: ${props => getColor(props.color)};
`;

const hoveredCss = css`
    ${colorCss};
    text-decoration: none;
    border-bottom: 1px dotted;
`;

const visitedCss = css`
    ${colorCss};
`;

const dottedCss = css`
    border-bottom: 1px dotted;
`;
const ExpanderDownIcon = ({ isSemitransparent, dropdownType, ...props }) => <Icons.ExpanderDownIcon {...props} />;

const Caret = styled(ExpanderDownIcon)`
    width: 10px;
    min-width: 10px;
    height: 10px;
    min-height: 10px;

    margin-left: 5px;
    margin-top: -4px;
    ${opacityCss};
    ${props => (props.dropdownType === 'appearDottedAfterHover') && `opacity: 0`};
    ${props => (props.dropdownType === 'appearDottedAfterHover') && `position: absolute`};

    path {
        fill: ${props => getColor(props.color)};
    }
`;

const StyledLinkWithDropdown = styled(SimpleLinkWithDropdown)`
    ${opacityCss};
    text-decoration: none;
    user-select: none;
        &:hover { 
            ${hoveredCss};
        }
        &:visited { 
            ${visitedCss};
        }
        &:not([href]):not([tabindex]) {
            ${colorCss};
            text-decoration: none;
            &:hover {
                ${hoveredCss};
            }
        }

${props => (props.dropdownType === 'alwaysDotted' && dottedCss)};

`;

const StyledSpan = styled.span`
    cursor: pointer;
    :hover {
        svg {  
        ${props => (props.dropdownType === 'appearDottedAfterHover' && `position: static`)};
        ${props => (props.dropdownType === 'appearDottedAfterHover' && `opacity: 1`)};
        ${props => (props.isSemitransparent && `opacity: 0.5`)};
        }
    }
`;

const SimpleText = ({ color, fontSize, isTextOverflow, ...props }) => <Text.Body {...props} />
const StyledText = styled(SimpleText)`
    ${colorCss};
    font-size: ${props => props.fontSize}px;

    ${props => (props.isTextOverflow && css`
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    `)}
`;

const DataDropDown = ({ data, color, fontSize, ...props }) => <DropDown {...props}></DropDown>;

class LinkWithDropdown extends React.PureComponent {

    constructor(props) {
        super(props);

        this.ref = React.createRef();

        this.state = {
            isOpen: false,
            data: props.data
        };

        this.handleClick = this.handleClick.bind(this);
        this.stopAction = this.stopAction.bind(this);
        this.toggleDropdown = this.toggleDropdown.bind(this);
        this.onDropDownItemClick = this.onDropDownItemClick.bind(this);

        if (props.isOpen)
            handleAnyClick(true, this.handleClick);
    }

    handleClick = (e) => this.state.isOpen && !this.ref.current.contains(e.target) && this.toggleDropdown(false);
    stopAction = (e) => !this.props.href && e.preventDefault();
    toggleDropdown = (isOpen) => this.setState({ isOpen: isOpen });
    clickToDropdown = () => {
        this.setState({
            data: this.props.data,
            isOpen: !this.state.isOpen
        });
    }

    componentWillUnmount() {
        handleAnyClick(false, this.handleClick);
    }

    componentDidUpdate(prevProps, prevState) {
        if (this.props.dropdownType !== prevProps.dropdownType) {
            if (this.props.isOpen !== prevProps.isOpen) {
                this.toggleDropdown(this.props.isOpen);
            }
        }
        else if (this.props.isOpen !== prevProps.isOpen) {
            this.toggleDropdown(this.props.isOpen);
        }

        if (this.state.isOpen !== prevState.isOpen) {
            handleAnyClick(this.state.isOpen, this.handleClick);
        }
    }

    onDropDownItemClick = (item) => {
        item.onClick && item.onClick();
        this.toggleDropdown(!this.state.isOpen);
    }

    render() {
        // console.log("LinkWithDropdown render");
        return (
            <>
                <StyledSpan
                    ref={this.ref}
                    isSemitransparent={this.props.isSemitransparent}
                    onClick={this.clickToDropdown}
                    dropdownType={this.props.dropdownType}
                >
                    <StyledLinkWithDropdown {...this.props}>
                        <StyledText
                            isTextOverflow={this.props.isTextOverflow}
                            fontSize={this.props.fontSize}
                            color={this.props.color}
                            isBold={this.props.isBold}
                            title={this.props.title}
                            tag='span'
                        >
                            {this.props.children}
                        </StyledText>
                    </StyledLinkWithDropdown>
                    <Caret
                        isSemitransparent={this.props.isSemitransparent}
                        color={this.props.color}
                        dropdownType={this.props.dropdownType}
                    />
                </StyledSpan>
                <DataDropDown isOpen={this.state.isOpen} {...this.props}>
                    {
                        this.state.data.map(item =>
                            <DropDownItem
                                {...item}
                                onClick={this.onDropDownItemClick.bind(this.props, item)}
                            />
                        )
                    }
                </DataDropDown>
            </>

        );
    };
}

LinkWithDropdown.propTypes = {
    color: PropTypes.oneOf(['gray', 'black', 'blue']),
    data: PropTypes.array,
    dropdownType: PropTypes.oneOf(['alwaysDotted', 'appearDottedAfterHover']).isRequired,
    fontSize: PropTypes.number,
    isBold: PropTypes.bool,
    isSemitransparent: PropTypes.bool,
    isTextOverflow: PropTypes.bool,
    title: PropTypes.string,
};

LinkWithDropdown.defaultProps = {
    color: 'black',
    data: [],
    dropdownType: 'alwaysDotted',
    fontSize: 12,
    isBold: false,
    isSemitransparent: false,
    isTextOverflow: true,
}

export default LinkWithDropdown;
