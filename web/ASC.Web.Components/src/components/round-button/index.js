import React, { useState, useEffect, useRef } from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import DropDown from '../drop-down'
import GroupButton from '../group-button'

const
  mainColor = '#cecece',
  activeColor = '#3b72a7';

const StyledOuther = styled.div`
  display: inline-block;
  position: relative;
`;

const StyledRoundButton = styled.div`
  border: 3px solid ${props => props.opened ? activeColor : mainColor};
  border-radius: 14px;
  -moz-border-radius: 14px;
  -webkit-border-radius: 14px;
  cursor: pointer;
  display: block;
  height: 14px;
  width: 14px;
  padding: 0;
  position: relative;
  box-sizing: content-box;

  &:after {
    border-left: 4px solid transparent;
    border-right: 4px solid transparent;
    border-top: 5px solid ${props => props.opened ? activeColor : mainColor};
    content: "";
    height: 0;
    width: 0;
    position: absolute;
    top: 50%;
    left: 50%;
    margin-left: -4px;
    margin-top: -2px;
  }

  &:hover {
    border-color: ${activeColor};

    &:after {
      border-top-color: ${activeColor};
    }
  }
`;

const loop = data => {
  return data.map((item) => {
    return <GroupButton {...item}></GroupButton>;
  });
};

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

const RoundButton = (props) => {
  const { data, opened } = props;
  const [isOpen, toggle] = useState(opened);
  const ref = useRef(null);
  
  useOuterClickNotifier((e) => toggle(false), ref);

  return (
      <StyledOuther onClick={() => { toggle(!isOpen) }} ref={ref} >
        <StyledRoundButton {...props} opened={isOpen}/>
        <DropDown {...props} isOpen={isOpen}>
          {loop(data)}
        </DropDown>
      </StyledOuther>
  );
}

RoundButton.propTypes = {
  title: PropTypes.string,
  opened: PropTypes.bool,
  data: PropTypes.array
};

RoundButton.defaultProps = {
  title: '',
  opened: false,
  data: []
};

export default RoundButton