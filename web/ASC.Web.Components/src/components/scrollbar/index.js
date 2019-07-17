import React from 'react'
import { Scrollbars } from 'react-custom-scrollbars';


const Scrollbar = (props) => {
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
      thumbV: { backgroundColor: 'rgba(0, 0, 0, 0.1)', width: '6px', borderRadius: 'inherit' },
      thumbH: { backgroundColor: 'rgba(0, 0, 0, 0.1)', height: '6px', borderRadius: 'inherit' }
    },
  };
  
  const stype = scrollbarType[props.stype || "smallBlack"];

  const thumbV = stype ? stype.thumbV : {};
  const thumbH = stype ? stype.thumbH : {};

  const renderNavThumbVertical = ({ style, ...props }) => (
    <div {...props} style={{ ...style, ...thumbV }} />
  );

  const renderNavThumbHorizontal = ({ style, ...props }) => (
    <div {...props} style={{ ...style, ...thumbH }} />
  );

  return (
    <Scrollbars renderThumbVertical={renderNavThumbVertical} renderThumbHorizontal={renderNavThumbHorizontal} {...props}/>
  );
}

export default Scrollbar;