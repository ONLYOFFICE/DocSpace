import React, { useState, useEffect, useRef } from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import DropDownItem from '../drop-down-item'
import DropDown from '../drop-down'
import { Icons } from '../icons'

const StyledOuther = styled.div`
  display: inline-block;
  position: relative;
`;

const useOuterClickNotifier = (onOuterClick, ref) => {
  useEffect(() => { 
      const handleClick = (e) => !ref.current.contains(e.target) && onOuterClick(e);

      if (ref.current) {
          document.addEventListener("click", handleClick);
      }

      return () => document.removeEventListener("click", handleClick);
  },
  [onOuterClick, ref]
  );
}

const ContextMenuButton = (props) => {

  const [data, setState] = useState(props.data);
  const [isOpen, toggle] = useState(props.opened);
  const ref = useRef(null);
  
  useOuterClickNotifier((e) => { toggle(false) }, ref);

  return (
      <StyledOuther onClick={() => { setState(props.getData()); toggle(!isOpen) }} ref={ref} >
        <Icons.VerticalDotsIcon size={props.size} color={props.color} />
        <DropDown {...props} isOpen={isOpen}>
          {
            data.map(item => <DropDownItem {...item}/>)
          }
        </DropDown>
      </StyledOuther>
  );
}

ContextMenuButton.propTypes = {
  opened: PropTypes.bool,
  data: PropTypes.array,
  getData: PropTypes.func.isRequired,
  color: PropTypes.string
};

ContextMenuButton.defaultProps = {
  opened: false,
  data: [],
  size: 'medium',
  color: '#999'
};

export default ContextMenuButton