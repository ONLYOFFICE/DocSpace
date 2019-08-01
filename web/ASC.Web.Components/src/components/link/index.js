import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Icons } from '../icons';
import DropDown from '../drop-down';
import DropDownItem from '../drop-down-item';

const SimpleLink = ({ rel, isBold, fontSize, isTextOverflow,
    isHovered, isSemitransparent, type, color, target,
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
    opacity: ${props =>
        (props.isSemitransparent && '0.5')};
`;

const colorCss = css`
    color: ${props => getColor(props.color)};
`;

const hoveredCss = css`
    ${colorCss};
    border-bottom: ${props => (props.type === 'action' ? '1px dotted;' : 'none')};
    text-decoration: ${props => (props.type === 'page' ? 'underline' : 'none')};
`;

const visitedCss = css`
    ${colorCss};
`;

const dottedCss = css`
    border-bottom: 1px dotted;
`;
const ExpanderDownIcon = ({ isSemitransparent, dropdownType, type, ...props }) => <Icons.ExpanderDownIcon {...props} />;

const Caret = styled(ExpanderDownIcon)`
    width: 10px;
    margin-left: 5px;
    margin-top: -4px;
    ${opacityCss};
    opacity: ${props => (props.type === 'action' && props.dropdownType === 'appearDottedAfterHover') && '0'};
    display: ${props => !(props.type === 'action' && props.dropdownType !== 'none') && 'none'};
    position: ${props => (props.type === 'action' && props.dropdownType === 'appearDottedAfterHover') && 'absolute'};

    path {
        fill: ${props => getColor(props.color)};
    }

`;

const StyledLink = styled(SimpleLink).attrs((props) => ({
    href: props.href,
    target: props.target,
    rel: props.target === '_blank' && 'noopener noreferrer',
    title: props.title
}))`
    ${colorCss};
    ${opacityCss};
    font-size: ${props => props.fontSize}px;
    position: relative;
    text-decoration: none;
    font-weight: ${props => (props.isBold && 'bold')};
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

${props => (props.isHovered && hoveredCss)}

${props => (props.type === 'action' &&
        (props.isHovered || props.dropdownType === 'alwaysDotted') &&
        dottedCss)}

${props => (props.isTextOverflow && css`
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
    `)}

`;

const StyledSpan = styled.span`
    cursor: pointer;

    :hover {
        svg {  
        position: static;
        opacity: ${props => (props.isSemitransparent ? '0.5' : '1')};
        }
    }
`;

const DataDropDown = ({ data, type, color, fontSize, ...props }) => <DropDown {...props}></DropDown>;

class Link extends React.PureComponent {

    constructor(props) {
        super(props);

        this.ref = React.createRef();

        this.state = {
            isOpen: false,
            isHovered: props.isHovered,
            isDropdown: props.dropdownType != 'none',
            data: props.data
        };

        this.handleClick = this.handleClick.bind(this);
        this.stopAction = this.stopAction.bind(this);
        this.toggleDropdown = this.toggleDropdown.bind(this);
        this.onDropDownItemClick = this.onDropDownItemClick.bind(this);
    }

    handleClick = (e) => !this.ref.current.contains(e.target) && this.toggleDropdown(false);
    stopAction = (e) => !this.props.href && e.preventDefault();
    toggleDropdown = (isOpen) => this.setState({ isOpen: isOpen });
    clickToDropdown = () => {
        this.setState({
            data: this.props.data,
            isOpen: !this.state.isOpen
        });
    }
    clickAction = (e) => {
        this.stopAction(e);
        this.props.hasOwnProperty("onClick") && this.props.onClick(e);
    }

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
        if (this.props.dropdownType !== prevProps.dropdownType) {
            if (this.props.isOpen !== prevProps.isOpen) {
                this.setState({
                    isDropdown: this.props.dropdownType != 'none',
                    isOpen: this.props.isOpen
                });
            }
            else {
                this.setState({ isDropdown: this.props.dropdownType != 'none' });
            }
        }
        else if (this.props.isOpen !== prevProps.isOpen) {
            this.toggleDropdown(this.props.isOpen);
        }
    }

    onDropDownItemClick = (item) => {
        item.onClick && item.onClick();
        this.toggleDropdown(!this.state.isOpen);
    }

    render() {
        console.log("Link render");
        return (
            <>
                <StyledSpan
                    ref={this.ref}
                    isSemitransparent={this.props.isSemitransparent}
                    onClick={
                        this.state.isDropdown ?
                            this.clickToDropdown
                            : this.clickAction
                    }>
                    <StyledLink {...this.props}>
                        {this.props.children}
                    </StyledLink>

                    <Caret
                        isSemitransparent={this.props.isSemitransparent}
                        size='small'
                        isfill={true}
                        color={this.props.color}
                        type={this.props.type}
                        dropdownType={this.props.dropdownType}
                    />

                </StyledSpan>

                {this.state.isDropdown &&
                    <DataDropDown isOpen={this.state.isOpen} {...this.props}>
                        {
                            this.state.data.map(item =>
                                <DropDownItem
                                    {...item}
                                    onClick={this.onDropDownItemClick.bind(this.props, item)}
                                />
                            )
                        }
                    </DataDropDown>}
            </>

        );
    };
}

Link.propTypes = {
    color: PropTypes.oneOf(['gray', 'black', 'blue']),
    data: PropTypes.array,
    dropdownType: PropTypes.oneOf(['alwaysDotted', 'appearDottedAfterHover', 'none']),
    fontSize: PropTypes.number,
    href: PropTypes.string,
    isBold: PropTypes.bool,
    isHovered: PropTypes.bool,
    isSemitransparent: PropTypes.bool,
    isTextOverflow: PropTypes.bool,
    onClick: PropTypes.func,
    target: PropTypes.oneOf(['_blank', '_self', '_parent', '_top']),
    text: PropTypes.string,
    title: PropTypes.string,
    type: PropTypes.oneOf(['action', 'page'])
};

Link.defaultProps = {
    color: 'black',
    data: [],
    dropdownType: 'none',
    fontSize: 12,
    href: undefined,
    isBold: false,
    isHovered: false,
    isSemitransparent: false,
    isTextOverflow: true,
    type: 'page'
}

export default Link;
