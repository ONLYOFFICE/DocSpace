/* eslint-disable react/display-name */
import React, { memo } from "react";
import styled, { css } from "styled-components";
import PropTypes from "prop-types";
import { FixedSizeList as List, areEqual } from "react-window";
import AutoSizer from "react-virtualized-auto-sizer";
import Heading from "@appserver/components/heading";
import ContextMenu from "@appserver/components/context-menu";
import CustomScrollbarsVirtualList from "@appserver/components/scrollbar";
import { tablet, desktop } from "@appserver/components/utils/device";

const paddingCss = css`
  @media ${desktop} {
    margin-left: 1px;
    padding-right: 3px;
  }

  @media ${tablet} {
    margin-left: -1px;
  }
`;

const StyledGridWrapper = styled.div`
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(216px, 1fr));
  width: 100%;
  margin-bottom: ${(props) => (props.isFolders ? "23px" : 0)};
  box-sizing: border-box;
  ${paddingCss};

  grid-gap: 14px 16px;

  @media ${tablet} {
    grid-gap: 14px;
  }
`;

const StyledTileContainer = styled.div`
  position: relative;

  .tile-item-wrapper {
    position: relative;
    width: 100%;

    &.file {
      padding: 0;
    }
    &.folder {
      padding: 0;
    }
  }

  .tile-items-heading {
    margin: 0;
    margin-bottom: 15px;
    pointer-events: none;
  }

  @media ${tablet} {
    margin-right: -3px;
  }
`;

class TileContainer extends React.PureComponent {
  constructor(props) {
    super(props);

    this.state = {
      contextOptions: [],
    };
  }

  onRowContextClick = (options) => {
    if (Array.isArray(options)) {
      this.setState({
        contextOptions: options,
      });
    }
  };

  componentDidMount() {
    window.addEventListener("contextmenu", this.onRowContextClick);
  }

  componentWillUnmount() {
    window.removeEventListener("contextmenu", this.onRowContextClick);
  }

  renderFolders = () => {
    return <div />;
  };

  renderFiles = () => {
    return <div />;
  };

  // eslint-disable-next-line react/prop-types
  renderTile = memo(({ data, index, style }) => {
    // eslint-disable-next-line react/prop-types
    const options = data[index].props.contextOptions;

    return (
      <div
        onContextMenu={this.onRowContextClick.bind(this, options)}
        style={style}
      >
        {data[index]}
      </div>
    );
  }, areEqual);

  render() {
    const {
      itemHeight,
      children,
      useReactWindow,
      id,
      className,
      style,
      headingFolders,
      headingFiles,
    } = this.props;

    const Folders = [];
    const Files = [];

    React.Children.map(children, (item, index) => {
      const { isFolder, fileExst, id } = item.props.item;
      if ((isFolder || id === -1) && !fileExst) {
        Folders.push(
          <div
            className="tile-item-wrapper folder"
            key={index}
            onContextMenu={this.onRowContextClick.bind(
              this,
              item.props.contextOptions
            )}
          >
            {item}
          </div>
        );
      } else {
        Files.push(
          <div
            className="tile-item-wrapper file"
            key={index}
            onContextMenu={this.onRowContextClick.bind(
              this,
              item.props.contextOptions
            )}
          >
            {item}
          </div>
        );
      }
    });

    const renderList = ({ height, width }) => (
      <List
        className="list"
        height={height}
        width={width}
        itemSize={itemHeight}
        itemCount={children.length}
        itemData={children}
        outerElementType={CustomScrollbarsVirtualList}
      >
        {this.renderTile}
      </List>
    );

    return (
      <StyledTileContainer
        id={id}
        className={className}
        style={style}
        useReactWindow={useReactWindow}
      >
        {Folders.length > 0 && (
          <>
            <Heading size="xsmall" className="tile-items-heading">
              {headingFolders}
            </Heading>
            {useReactWindow ? (
              <AutoSizer>{renderList}</AutoSizer>
            ) : (
              <StyledGridWrapper isFolders>{Folders}</StyledGridWrapper>
            )}
          </>
        )}

        {Files.length > 0 && (
          <>
            {Folders.length > 0 && (
              <Heading size="xsmall" className="tile-items-heading">
                {headingFiles}
              </Heading>
            )}
            {useReactWindow ? (
              <AutoSizer>{renderList}</AutoSizer>
            ) : (
              <StyledGridWrapper>{Files}</StyledGridWrapper>
            )}
          </>
        )}

        <ContextMenu targetAreaId={id} options={this.state.contextOptions} />
      </StyledTileContainer>
    );
  }
}

TileContainer.propTypes = {
  itemHeight: PropTypes.number,
  manualHeight: PropTypes.string,
  children: PropTypes.any.isRequired,
  useReactWindow: PropTypes.bool,
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array]),
};

TileContainer.defaultProps = {
  itemHeight: 50,
  useReactWindow: true,
  id: "rowContainer",
};

export default TileContainer;
