import React, { memo } from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import CustomScrollbarsVirtualList from '../scrollbar/custom-scrollbars-virtual-list'
import DropDownItem from '../drop-down-item'
import { FixedSizeList } from "react-window"

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
    z-index: 1000;
    margin-top: ${props => (props.isUserPreview ? '6px' : '0px')};
    margin-right: ${props => (props.isUserPreview ? '6px' : '0px')};
    display: ${props => (props.isOpen || props.opened ? 'block' : 'none')};
    background: #FFFFFF;
    border-radius: 6px;
    -moz-border-radius: 6px;
    -webkit-border-radius: 6px;
    box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -moz-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
    -webkit-box-shadow: 0px 5px 20px rgba(0, 0, 0, 0.13);
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

const Row = memo(({ data, index, style }) => {
  const option = data[index];

  return (
    <DropDownItem
      {...option.props}
      style={style} />
  );
});

class DropDown extends React.PureComponent {

  constructor(props) {
    super(props);

    this.state = {
      width: 100
    };
    
    this.dropDown = React.createRef();
  };

  setDropDownWidthState = () => {
    if (this.dropDown.current) {
      this.setState({
        width: this.dropDown.current.offsetWidth
      });
    }
  }

  componentDidMount () {
    this.setDropDownWidthState();
  };

  componentDidUpdate(prevProps) {
    if (this.props.opened !== prevProps.opened || this.props.isOpen !== prevProps.isOpen) {
      this.setDropDownWidthState();
    }
  };

  render() {
    const {maxHeight, withArrow, directionX, children} = this.props;
    const dropDownMaxHeightProp = maxHeight ? { height: maxHeight + 'px' } : {};
    //console.log("DropDown render");
    return (
      <StyledDropdown
        ref={this.dropDown}
        {...this.props}
        {...dropDownMaxHeightProp}
      >
        {withArrow && <Arrow directionX={directionX} />}
        {maxHeight
          ? <FixedSizeList
            height={maxHeight}
            width={this.state.width}
            itemSize={36}
            itemCount={children.length}
            itemData={children}
            outerElementType={CustomScrollbarsVirtualList}
          >
            {Row}
          </FixedSizeList>
          : children}
      </StyledDropdown>
    );
  }
};

DropDown.propTypes = {
  directionX: PropTypes.oneOf(['left', 'right']),
  directionY: PropTypes.oneOf(['bottom', 'top']),
  withArrow: PropTypes.bool,
  manualWidth: PropTypes.string,
  manualY: PropTypes.string,
  manualX: PropTypes.string,
  maxHeight: PropTypes.number
};

DropDown.defaultProps = {
  directionX: 'left',
  directionY: 'bottom',
  withArrow: false
};

export default DropDown