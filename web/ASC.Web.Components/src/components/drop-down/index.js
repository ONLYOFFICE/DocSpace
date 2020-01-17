import React, { memo } from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import CustomScrollbarsVirtualList from '../scrollbar/custom-scrollbars-virtual-list'
import DropDownItem from '../drop-down-item'
import Backdrop from '../backdrop'
import { FixedSizeList } from "react-window"
import onClickOutside from "react-onclickoutside";

const StyledDropdown = styled.div`
    font-family: 'Open Sans',sans-serif,Arial;
    font-style: normal;
    font-weight: 600;
    font-size: 13px;

    ${props => props.maxHeight && `
      max-height: ${props.maxHeight}px;
      overflow-y: auto;
    `}

    position: absolute;
    ${props => props.manualWidth && `width: ${props.manualWidth};`}
    ${props => (props.directionY === 'top' && css`bottom: ${props => props.manualY ? props.manualY : '100%'};`)}
    ${props => (props.directionY === 'bottom' && css`top: ${props => props.manualY ? props.manualY : '100%'};`)}
    ${props => (props.directionX === 'right' && css`right: ${props => props.manualX ? props.manualX : '0px'};`)}
    ${props => (props.directionX === 'left' && css`left: ${props => props.manualX ? props.manualX : '0px'};`)}
    z-index: 150;
    margin-top: ${props => (props.isUserPreview ? '6px' : '0px')};
    margin-right: ${props => (props.isUserPreview ? '6px' : '0px')};
    display: ${props => (props.open ? 'block' : 'none')};
    background: #FFFFFF;
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);

    padding: ${props => !props.maxHeight && `6px 0px`};
`;

const Arrow = styled.div`
    position: absolute;
    top: -6px;
    ${props => (props.directionX === 'right' && css`right: 16px;`)}
    ${props => (props.directionX === 'left' && css`left: 16px;`)}
    width: 24px;
    height: 6px;
    background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M9.27954 1.12012C10.8122 -0.295972 13.1759 -0.295971 14.7086 1.12012L18.8406 4.93793C19.5796 5.62078 20.5489 6 21.5551 6H24H0H2.43299C3.4392 6 4.40845 5.62077 5.1475 4.93793L9.27954 1.12012Z' fill='%23206FA4'/%3E%3C/svg%3E");
`;

// eslint-disable-next-line react/display-name, react/prop-types
const Row = memo(({ data, index, style }) => {
  const option = data[index];

  return (
    <DropDownItem
      // eslint-disable-next-line react/prop-types
      {...option.props}
      style={style} />
  );
});

class DropDown extends React.PureComponent {

  constructor(props) {
    super(props);

    this.state = {
      width: this.dropDownRef ? this.dropDownRef.current.offsetWidth : 240,
      directionX: props.directionX,
      directionY: props.directionY
    };

    this.dropDownRef = React.createRef();
  }

  componentDidMount() {
    if (this.props.open) {
      this.props.enableOnClickOutside();
      this.checkPosition();
    }
  }

  componentWillUnmount() {
    this.props.disableOnClickOutside();
  }

  componentDidUpdate(prevProps) {
    if (this.props.open !== prevProps.open) {
      if (this.props.open) {
        this.props.enableOnClickOutside();
        this.checkPosition();
      }
      else {
        this.props.disableOnClickOutside();
      }

    }
  }

  handleClickOutside = e => {
    //console.log(`DropDown handleClickOutside`, e);
    this.toggleDropDown(e);
  };

  toggleDropDown = (e) => {
    this.props.clickOutsideAction && this.props.clickOutsideAction(e, !this.props.open);
  }

  checkPosition = () => {
    if (!this.dropDownRef.current) return;

    const rects = this.dropDownRef.current.getBoundingClientRect();
    const container = { width: window.innerWidth, height: window.innerHeight };

    const left = rects.left < 0;
    const right = rects.right > container.width;

    let newDirection = {};

    newDirection.directionX = left ? 'left' : right ? 'right' : this.props.directionX;

    this.setState({
      directionX: newDirection.directionX,
      width: rects.width
    });
  }

  render() {
    const { maxHeight, withArrow, withBackdrop, children, open } = this.props;
    const { directionX, directionY, width } = this.state;
    const isTablet = window.innerWidth < 1024; //TODO: Make some better
    const itemHeight = isTablet ? 36 : 32;
    const fullHeight = children && children.length * itemHeight;
    const calculatedHeight = ((fullHeight > 0) && (fullHeight < maxHeight)) ? fullHeight : maxHeight;
    const dropDownMaxHeightProp = maxHeight ? { height: calculatedHeight + 'px' } : {};
    //console.log("DropDown render");
    return (
      <>
        <StyledDropdown
          ref={this.dropDownRef}
          {...this.props}
          directionX={directionX}
          directionY={directionY}
          {...dropDownMaxHeightProp}
        >
          {withArrow && <Arrow directionX={directionX} />}
          {maxHeight
            ? <FixedSizeList
              height={calculatedHeight}
              width={width}
              itemSize={itemHeight}
              itemCount={children.length}
              itemData={children}
              outerElementType={CustomScrollbarsVirtualList}
            >
              {Row}
            </FixedSizeList>
            : children}
        </StyledDropdown>
        {(withBackdrop && open && isTablet) && <Backdrop visible zIndex={149} onClick={this.toggleDropDown} />}
      </>
    );
  }
}

DropDown.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  directionX: PropTypes.oneOf(['left', 'right']),
  directionY: PropTypes.oneOf(['bottom', 'top']),
  id: PropTypes.string,
  open: PropTypes.bool,
  manualWidth: PropTypes.string,
  manualX: PropTypes.string,
  manualY: PropTypes.string,
  maxHeight: PropTypes.number,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
  withArrow: PropTypes.bool,
  withBackdrop: PropTypes.bool,
  clickOutsideAction: PropTypes.func,
  enableOnClickOutside: PropTypes.func,
  disableOnClickOutside: PropTypes.func
};

DropDown.defaultProps = {
  directionX: 'left',
  directionY: 'bottom',
  withArrow: false,
  withBackdrop: true
};

const EnhancedComponent = onClickOutside(DropDown);

class DropDownContainer extends React.Component {
  render() {
    return <EnhancedComponent disableOnClickOutside={true} {...this.props} />;
  }
}

export default DropDownContainer;
