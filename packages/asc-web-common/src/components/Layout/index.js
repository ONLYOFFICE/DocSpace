import React, { useEffect } from "react";
import styled from "styled-components";
import { utils } from "@appserver/components/src";
import { connect } from "react-redux";
import store from "../../store";

const { setIsTabletView } = store.auth.actions;
const { getIsTabletView } = store.auth.selectors;

const { size } = utils.device;
const StyledContainer = styled.div``;

const Layout = (props) => {
  const { children, isTabletView, setIsTabletView } = props;

  useEffect(() => {
    const isTablet = window.innerWidth <= size.tablet;
    setIsTabletView(isTablet);

    let mediaQuery = window.matchMedia("(max-width: 1024px)");
    mediaQuery.addListener(isViewChangeHandler);

    return () => mediaQuery.removeListener(isViewChangeHandler);
  }, []);

  const isViewChangeHandler = (e) => {
    const { matches } = e;
    setIsTabletView(matches);
  };

  return (
    <StyledContainer className="Layout" isTabletView={isTabletView}>
      {children}
    </StyledContainer>
  );
};

const mapStateToProps = (state) => {
  return {
    isTabletView: getIsTabletView(state),
  };
};

const mapDispatchToProps = (dispatch) => {
  return {
    setIsTabletView: (isTabletView) => dispatch(setIsTabletView(isTabletView)),
  };
};
export default connect(mapStateToProps, mapDispatchToProps)(Layout);
