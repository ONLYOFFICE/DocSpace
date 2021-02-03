import React, { useEffect } from "react";
import PropTypes from "prop-types";
import styled from "styled-components";
import { utils } from "asc-web-components";
import { inject, observer } from "mobx-react";

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

Layout.propTypes = {
  isTabletView: PropTypes.bool,
  children: PropTypes.any,
  setIsTabletView: PropTypes.func,
};

export default inject(({ store }) => {
  return {
    isTabletView: store.settingsStore.isTabletView,
    setIsTabletView: (isTablet) => {
      store.settingsStore.isTabletView = isTablet;
    },
  };
})(observer(Layout));
