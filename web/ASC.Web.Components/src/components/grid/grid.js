import React, {forwardRef} from "react";
import PropTypes from 'prop-types';
import StyledGrid from './styledGrid';

const Grid = forwardRef(({ tag, as, ...rest }, ref) => {
  //console.log("Grid render", rest)
  return (
    <StyledGrid as={!as && tag ? tag : as} ref={ref} {...rest} />
  );
});

Grid.propTypes = {
  as: PropTypes.string,
  alignContent: PropTypes.string,
  alignItems: PropTypes.string,
  alignSelf: PropTypes.string,
  areasProp: PropTypes.array,
  columnsProp: PropTypes.oneOfType([PropTypes.string, PropTypes.array, PropTypes.object]),
  gridArea: PropTypes.string,
  gridColumnGap: PropTypes.string,
  gridGap: PropTypes.string,
  gridRowGap: PropTypes.string,
  heightProp: PropTypes.string,
  justifyContent: PropTypes.string,
  justifyItems: PropTypes.string,
  justifySelf: PropTypes.string,
  marginProp: PropTypes.string,
  paddingProp: PropTypes.string,
  rowsProp: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  tag: PropTypes.string,
  widthProp: PropTypes.string
}

Grid.defaultProps = {
  heightProp: '100%',
  widthProp: '100%'
};

Grid.displayName = "Grid";

export default Grid;