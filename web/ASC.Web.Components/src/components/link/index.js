import React from 'react';
import styled, { css } from 'styled-components';
import PropTypes from 'prop-types';
import { Icons } from '../icons'
import DropDown from '../drop-down'

const SimpleLink = ({ rel, isBold, fontSize, isTextOverflow,
    isHovered, isSemitransparent, type, color, text, target,
    dropdownType, ...props }) => <a {...props}>{text}</a>;

const getDropdownColor = color => {
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
    color: ${props => getDropdownColor(props.color)};
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

const Caret = styled(Icons.ExpanderDownIcon).attrs((props) => ({
    isSemitransparent: props.isSemitransparent
}))`
    width: 10px;
    margin-left: 5px;
    margin-top: -4px;
    ${opacityCss};
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
    cursor: pointer;
    position: relative;
    text-decoration: none;
    font-weight: ${props => (props.isBold && 'bold')};
    
    user-select: none;
    -o-user-select: none;
    -moz-user-select: none;
    -webkit-user-select: none;

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
        -o-text-overflow: ellipsis;
        -moz-text-overflow: ellipsis;
        -webkit-text-overflow: ellipsis;
    `)}

`;

class Link extends React.Component {

    constructor(props) {
        super(props);

        this.ref = React.createRef();

        this.state = {
            isOpen: false,
            isHovered: props.isHovered,
            isDropdown: props.dropdownType != 'none'
        };
    }

    handleClick = (e) => !this.ref.current.contains(e.target) && this.toggleDropdown(false);
    stopAction = (e) => !this.props.href && e.preventDefault();
    toggleDropdown = (isOpen) => this.setState({ isOpen: isOpen });
    toggleHovered = (isHovered) => this.setState({ isHovered: isHovered });

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
            this.setState({isDropdown: this.props.dropdownType != 'none'});
        }
    }

    render() {
        return (
            <span ref={this.ref}
                onMouseEnter={() => {
                    this.props.dropdownType === 'appearDottedAfterHover' &&
                        this.toggleHovered(!this.state.isHovered)
                }}
                onMouseLeave={() => {
                    this.props.dropdownType === 'appearDottedAfterHover' &&
                        this.toggleHovered(!this.state.isHovered)
                }}>

                <StyledLink {...this.props} onClick={
                    this.state.isDropdown ?
                        () => { this.toggleDropdown(!this.state.isOpen) }
                        : this.stopAction}
                />
                {this.state.isDropdown &&
                    (this.state.isHovered || this.props.dropdownType === 'alwaysDotted') &&
                    <Caret
                        isSemitransparent={this.props.isSemitransparent}
                        size='small'
                        isfill={true}
                        color={getDropdownColor(this.props.color)} />
                }
                {this.state.isDropdown &&
                    <DropDown isOpen={this.state.isOpen} {...this.props}>
                        {this.props.children}
                    </DropDown>}
            </span>

        );
    };
}

Link.propTypes = {
    color: PropTypes.oneOf(['gray', 'black', 'blue']),
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
