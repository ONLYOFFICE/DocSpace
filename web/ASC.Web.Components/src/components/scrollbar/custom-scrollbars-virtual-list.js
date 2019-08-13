import React from 'react';
import Scrollbar from '../scrollbar';

class CustomScrollbars extends React.Component { 
    refSetter = (scrollbarsRef, forwardedRef) => {
      if (scrollbarsRef) {
        forwardedRef(scrollbarsRef.view);
      } else {
        forwardedRef(null);
      }
    };
  
    render() {
      const { onScroll, forwardedRef, style, children } = this.props;
    return (
      <Scrollbar
        ref={scrollbarsRef => this.refSetter.bind(this, scrollbarsRef, forwardedRef)}
        style={{ ...style, overflow: "hidden" }}
        onScroll={onScroll}
        stype="mediumBlack"
      >
        {children}
      </Scrollbar>
    );
    };
  };
  
  const CustomScrollbarsVirtualList = React.forwardRef((props, ref) => (
    <CustomScrollbars {...props} forwardedRef={ref} />
  ));

  export default CustomScrollbarsVirtualList;