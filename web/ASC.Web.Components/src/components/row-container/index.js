/* eslint-disable react/display-name */
import React, { memo } from 'react';
import styled from 'styled-components';
import PropTypes from 'prop-types';
import CustomScrollbarsVirtualList from '../scrollbar/custom-scrollbars-virtual-list';
import { FixedSizeList as List, areEqual } from 'react-window';
import AutoSizer from 'react-virtualized-auto-sizer';
import ContextMenu from '../context-menu';

const StyledRowContainer = styled.div`
  height: ${props => props.useReactWindow ? props.manualHeight ? props.manualHeight : '100%' : 'auto'};
  margin: 16px 0;
  position: relative;
`;

class RowContainer extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      contextOptions: []
    };
  }

  onRowContextClick = (options) => {
    if (Array.isArray(options)) {
      this.setState({
        contextOptions: options
      });
    }
  };

  componentDidMount() {
    window.addEventListener('contextmenu', this.onRowContextClick);
  }

  componentWillUnmount() {
    window.removeEventListener('contextmenu', this.onRowContextClick);
  }

  // eslint-disable-next-line react/prop-types
  renderRow = memo(({ data, index, style }) => {
    // eslint-disable-next-line react/prop-types
    const options = data[index].props.contextOptions;
    
    return (
      <div onContextMenu={this.onRowContextClick.bind(this, options)} style={style}>
        {data[index]}
      </div>
    )
  }, areEqual);

  render() {
    const { manualHeight, itemHeight, children, useReactWindow } = this.props;

    const renderList = ({ height, width }) => (
      <List
        className="List"
        height={height}
        width={width}
        itemSize={itemHeight}
        itemCount={children.length}
        itemData={children}
        outerElementType={CustomScrollbarsVirtualList}
      >
        {this.renderRow}
      </List>
    );

    return (
      <StyledRowContainer id="rowContainer" manualHeight={manualHeight} useReactWindow={useReactWindow}>
        { useReactWindow ? (
          <AutoSizer>{renderList}</AutoSizer>
        ) : (
          children.map((item, index) => (
            <div key={index} onContextMenu={this.onRowContextClick.bind(this, item.props.contextOptions)}>
              {item}
            </div>
          ))
        )}
        <ContextMenu targetAreaId="rowContainer" options={this.state.contextOptions} />
      </StyledRowContainer>
    );
  }
}

RowContainer.propTypes = {
  itemHeight: PropTypes.number,
  manualHeight: PropTypes.string,
  children: PropTypes.any.isRequired,
  useReactWindow: PropTypes.bool
};

RowContainer.defaultProps = {
  itemHeight: 50,
  useReactWindow: true
};

export default RowContainer;