import React from 'react'
import { Scrollbars } from 'react-custom-scrollbars';


const Scrollbar = React.forwardRef((props, ref) => {
  //console.log("Scrollbar render");
  const scrollbarType = {
    smallWhite: {
      thumbV: { backgroundColor: 'rgba(256, 256, 256, 0.2)', width: '2px', marginLeft: '2px', borderRadius: 'inherit' },
      thumbH: { backgroundColor: 'rgba(256, 256, 256, 0.2)', height: '2px', marginTop: '2px', borderRadius: 'inherit' }
    },
    smallBlack: {
      thumbV: { backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '2px', marginLeft: '2px', borderRadius: 'inherit' },
      thumbH: { backgroundColor: 'rgba(0, 0, 0, 0.1)', height: '2px', marginTop: '2px', borderRadius: 'inherit' }
    },
    mediumBlack: {
      thumbV: { backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '8px', borderRadius: 'inherit' },
      thumbH: { backgroundColor: 'rgba(0, 0, 0, 0.1)', height: '8px', borderRadius: 'inherit' }
    },
    preMediumBlack: {
      thumbV: { backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '5px', borderRadius: 'inherit', cursor: 'default' },
      thumbH: { backgroundColor: 'rgba(0, 0, 0, 0.1)', height: '5px', borderRadius: 'inherit', cursor: 'default' }
    },
  };
  
  const stype = scrollbarType[props.stype];

  const thumbV = stype ? stype.thumbV : {};
  const thumbH = stype ? stype.thumbH : {};

  const renderNavThumbVertical = ({ style, ...props }) => (
    <div {...props} style={{ ...style, ...thumbV }} />
  );

  const renderNavThumbHorizontal = ({ style, ...props }) => (
    <div {...props} style={{ ...style, ...thumbH }} />
  );

  return (
    <Scrollbars renderThumbVertical={renderNavThumbVertical} renderThumbHorizontal={renderNavThumbHorizontal} {...props} ref={ref} />
  );
});

Scrollbar.defaultProps = {
  stype: "smallBlack"
};

export default Scrollbar;