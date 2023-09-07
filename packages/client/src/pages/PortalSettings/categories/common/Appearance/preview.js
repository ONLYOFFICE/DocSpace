import PlusPreviewSvgUrl from "PUBLIC_DIR/images/plus.preview.svg?url";
import React, { useState, useEffect } from "react";
import Loaders from "@docspace/common/components/Loaders";
import ContextMenuButton from "@docspace/components/context-menu-button";
import { isTablet } from "react-device-detect";
import {
  StyledComponent,
  StyledFloatingButton,
  IconBox,
  StyledMobilePreview,
} from "./StyledPreview";

import ButtonPlusIcon from "PUBLIC_DIR/images/actions.button.plus.react.svg";
import { saveToSessionStorage, getFromSessionStorage } from "../../../utils";
import { isMobileOnly, isDesktop } from "react-device-detect";

const Preview = (props) => {
  const {
    appliedColorAccent,
    previewAccent,
    themePreview,
    selectThemeId,
    withBorder,
    withTileActions,
    floatingButtonClass,
    colorCheckImg,
  } = props;
  const [colorPreview, setColorPreview] = useState(previewAccent);
  const [isViewTablet, setIsViewTablet] = useState(false);
  const [isSmallWindow, setIsSmallWindow] = useState(false);

  const onCheckView = () => {
    const tablet =
      isTablet || (window.innerWidth > 600 && window.innerWidth <= 1024);
    setIsViewTablet(tablet);

    if (isDesktop && window.innerWidth < 600) {
      setIsSmallWindow(true);
    } else {
      setIsSmallWindow(false);
    }
  };

  const getSettings = () => {
    const selectColorAccent = getFromSessionStorage("selectColorAccent");
    saveToSessionStorage("defaultColorAccent", appliedColorAccent);

    if (selectColorAccent) {
      setColorPreview(selectColorAccent);
    } else {
      setColorPreview(appliedColorAccent);
    }
  };

  useEffect(() => {
    getSettings();
  }, [previewAccent]);

  useEffect(() => {
    saveToSessionStorage("selectColorAccent", colorPreview);
  }, [colorPreview]);

  useEffect(() => {
    onCheckView();
    window.addEventListener("resize", onCheckView);

    return () => {
      window.removeEventListener("resize", onCheckView);
    };
  });

  return isSmallWindow || isMobileOnly ? (
    <StyledMobilePreview
      selectThemeId={selectThemeId}
      themePreview={themePreview}
      colorPreview={colorPreview}
    >
      <div className="preview_mobile-header">
        <Loaders.Rectangle
          animate={false}
          width="20"
          height="20"
          className="loaders-theme"
        />
        <Loaders.Rectangle
          animate={false}
          height="24"
          className="loaders-theme"
        />
        <Loaders.Rectangle
          animate={false}
          width="32"
          height="32"
          borderRadius="50"
          className="loaders-theme-avatar"
        />
      </div>
      <div className="preview_mobile-navigation">
        <div className="header">
          <Loaders.Rectangle
            animate={false}
            height="24"
            className="loaders-theme"
          />
        </div>
      </div>
      <div className="section-search background border-color">
        <Loaders.Rectangle
          animate={false}
          width="48"
          height="12"
          className="loaders-theme loader-search"
        />
      </div>

      <div className="tile background border-color">
        <div className="tile-name">
          <div className="tile-container">
            <div className="tile-icon">
              <Loaders.Rectangle
                animate={false}
                width="32"
                height="32"
                className="loaders-tile-theme"
              />
            </div>

            <Loaders.Rectangle
              animate={false}
              width="48"
              height="10"
              borderRadius="3"
              className="loaders-tile-text-theme"
            />
          </div>

          {withTileActions && (
            <div className="action-button">
              <Loaders.Rectangle
                animate={false}
                width="16"
                height="16"
                borderRadius="50"
                className="section-badge color-badge"
              />
              <svg
                className="pin"
                width="12"
                height="16"
                viewBox="0 0 12 16"
                fill="none"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path d="M9.5783 -0.000242493L2.41936 -0.000241966C2.1608 -0.000374392 1.95098 0.209442 1.95111 0.468004L1.95111 0.498338C1.95105 0.937244 2.12199 1.35006 2.43234 1.66041C2.63229 1.86036 2.87496 2.00143 3.13948 2.07719L2.60851 7.27556C2.06569 7.30602 1.55963 7.53167 1.17212 7.91918C0.754536 8.33676 0.524586 8.8919 0.524652 9.48234L0.524652 9.52725C0.524586 9.78587 0.73427 9.99556 0.992898 9.99549L4.99937 9.99549L4.9993 13.5101C5.0005 13.5949 5.17017 15.0259 5.19137 15.2188C5.21258 15.4118 5.36324 15.5487 5.36417 15.5624C5.38013 15.8082 5.75521 15.9992 6.00178 15.9998C6.13093 16 6.41664 15.9478 6.50187 15.8625C6.57956 15.7848 6.63029 15.6798 6.63818 15.5624C6.6389 15.5521 6.79437 15.3697 6.81097 15.2188C6.82757 15.068 7.00165 13.595 7.00284 13.5036V9.99562L11.0049 9.99562C11.1342 9.99569 11.2513 9.94324 11.336 9.85853C11.4207 9.77382 11.4733 9.65666 11.4731 9.52738V9.48247C11.4732 8.89203 11.2432 8.33683 10.8257 7.91931C10.4382 7.5318 9.93203 7.30622 9.38928 7.27569L8.85831 2.07733C9.12283 2.00156 9.3655 1.86049 9.56545 1.66054C9.87587 1.35012 10.0468 0.937442 10.0467 0.49847L10.0467 0.468136C10.0466 0.209376 9.83692 -0.000308579 9.5783 -0.000242493Z" />
              </svg>
              <ContextMenuButton
                getData={() => {}}
                directionX="right"
                className="menu-button"
                isDisabled
              />
            </div>
          )}
        </div>
        <div className="tile-tag border-color">
          <Loaders.Rectangle
            animate={false}
            width="63"
            height="24"
            className="loaders-tile-theme"
          />
        </div>
      </div>
      <StyledFloatingButton
        className={
          floatingButtonClass
            ? `${floatingButtonClass} floating-button`
            : "floating-button"
        }
        colorPreview={colorPreview}
        themePreview={themePreview}
        selectThemeId={selectThemeId}
      >
        <IconBox
          colorPreview={colorPreview}
          themePreview={themePreview}
          selectThemeId={selectThemeId}
          colorCheckImg={colorCheckImg}
        >
          <ButtonPlusIcon />
        </IconBox>
      </StyledFloatingButton>
    </StyledMobilePreview>
  ) : (
    <StyledComponent
      colorPreview={colorPreview}
      themePreview={themePreview}
      selectThemeId={selectThemeId}
      isViewTablet={isViewTablet}
      withBorder={withBorder}
    >
      <div className="menu border-color">
        {!isViewTablet ? (
          <>
            <div className="header">
              <Loaders.Rectangle
                animate={false}
                width="211"
                height="24"
                className="loaders-theme"
              />
            </div>

            <div className="main-button-container">
              <Loaders.Rectangle
                animate={false}
                height="32"
                className="main-button-preview"
              />
            </div>

            <div className="menu-section">
              <div className="title-section">
                <Loaders.Rectangle
                  animate={false}
                  width="37"
                  height="12"
                  className="loaders-theme"
                />
              </div>

              <div className="flex">
                <div className="padding-right">
                  <Loaders.Rectangle
                    animate={false}
                    width="16"
                    height="16"
                    className="loaders-theme"
                  />
                </div>

                <Loaders.Rectangle
                  animate={false}
                  width="48"
                  height="8"
                  className="loaders-theme"
                />
              </div>
              <div className="flex select">
                <div className="padding-right">
                  <Loaders.Rectangle
                    animate={false}
                    width="16"
                    height="16"
                    className="color-loaders"
                  />
                </div>

                <Loaders.Rectangle
                  animate={false}
                  width="48"
                  height="8"
                  className="color-loaders"
                />
                <Loaders.Rectangle
                  animate={false}
                  width="22"
                  height="16"
                  borderRadius="8"
                  className="menu-badge color-badge"
                />
              </div>
              <div className="flex">
                <div className="padding-right">
                  <Loaders.Rectangle
                    animate={false}
                    width="16"
                    height="16"
                    className="loaders-theme"
                  />
                </div>
                <Loaders.Rectangle
                  animate={false}
                  width="48"
                  height="8"
                  className="loaders-theme"
                />
              </div>
            </div>

            <div className="menu-section">
              <div className="title-section">
                <Loaders.Rectangle
                  animate={false}
                  width="37"
                  height="12"
                  className="loaders-theme"
                />
              </div>

              <div className="flex">
                <div className="padding-right">
                  <Loaders.Rectangle
                    animate={false}
                    width="16"
                    height="16"
                    className="loaders-theme"
                  />
                </div>

                <Loaders.Rectangle
                  animate={false}
                  width="48"
                  height="8"
                  className="loaders-theme"
                />
              </div>
            </div>
          </>
        ) : (
          <>
            <Loaders.Rectangle
              animate={false}
              width="28"
              height="28"
              className="tablet-header"
            />
            <div className="line"></div>
            <Loaders.Rectangle
              animate={false}
              width="20"
              height="20"
              className="tablet-category"
            />
            <svg
              className="tablet-category-notice color-loaders"
              width="30"
              height="30"
              viewBox="0 0 30 30"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <rect x="4" y="6" width="20" height="20" rx="2" fill="#11A3D4" />
              <circle cx="24" cy="6" r="5" fill="#11A3D4" stroke="#F8F9F9" />
            </svg>
            <Loaders.Rectangle
              animate={false}
              width="20"
              height="20"
              className="tablet-category bottom"
            />
            <div className="line"></div>
            <Loaders.Rectangle
              animate={false}
              width="20"
              height="20"
              className="tablet-category"
            />
            <Loaders.Rectangle
              animate={false}
              className="tablet-category tablet-half"
            />
          </>
        )}
      </div>

      <div className="section border-color">
        <div className="section-header">
          <div className="section-header-loader">
            <Loaders.Rectangle
              animate={false}
              width="60"
              height="16"
              className="loaders-theme"
            />
          </div>

          <img src={PlusPreviewSvgUrl} />
        </div>
        <div className="section-search background border-color">
          <div className="section-search-loader">
            <Loaders.Rectangle
              animate={false}
              width="48"
              height="12"
              className="loaders-theme loader-search"
            />
          </div>
        </div>
        <div className="section-tile">
          <div className="section-flex-tablet">
            <div className="tile background border-color">
              <div className="tile-name">
                <div className="tile-container">
                  <div className="tile-icon">
                    <Loaders.Rectangle
                      animate={false}
                      width="32"
                      height="32"
                      className="loaders-theme"
                    />
                  </div>

                  <div className="tile-title">
                    <Loaders.Rectangle
                      animate={false}
                      width="48"
                      height="10"
                      className="loaders-theme"
                    />
                  </div>
                </div>

                {withTileActions && (
                  <div className="action-button">
                    <Loaders.Rectangle
                      animate={false}
                      width="16"
                      height="16"
                      borderRadius="50"
                      className="section-badge color-badge"
                    />
                    <svg
                      className="pin"
                      width="12"
                      height="16"
                      viewBox="0 0 12 16"
                      fill="none"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path d="M9.5783 -0.000242493L2.41936 -0.000241966C2.1608 -0.000374392 1.95098 0.209442 1.95111 0.468004L1.95111 0.498338C1.95105 0.937244 2.12199 1.35006 2.43234 1.66041C2.63229 1.86036 2.87496 2.00143 3.13948 2.07719L2.60851 7.27556C2.06569 7.30602 1.55963 7.53167 1.17212 7.91918C0.754536 8.33676 0.524586 8.8919 0.524652 9.48234L0.524652 9.52725C0.524586 9.78587 0.73427 9.99556 0.992898 9.99549L4.99937 9.99549L4.9993 13.5101C5.0005 13.5949 5.17017 15.0259 5.19137 15.2188C5.21258 15.4118 5.36324 15.5487 5.36417 15.5624C5.38013 15.8082 5.75521 15.9992 6.00178 15.9998C6.13093 16 6.41664 15.9478 6.50187 15.8625C6.57956 15.7848 6.63029 15.6798 6.63818 15.5624C6.6389 15.5521 6.79437 15.3697 6.81097 15.2188C6.82757 15.068 7.00165 13.595 7.00284 13.5036V9.99562L11.0049 9.99562C11.1342 9.99569 11.2513 9.94324 11.336 9.85853C11.4207 9.77382 11.4733 9.65666 11.4731 9.52738V9.48247C11.4732 8.89203 11.2432 8.33683 10.8257 7.91931C10.4382 7.5318 9.93203 7.30622 9.38928 7.27569L8.85831 2.07733C9.12283 2.00156 9.3655 1.86049 9.56545 1.66054C9.87587 1.35012 10.0468 0.937442 10.0467 0.49847L10.0467 0.468136C10.0466 0.209376 9.83692 -0.000308579 9.5783 -0.000242493Z" />
                    </svg>
                    <ContextMenuButton
                      getData={() => {}}
                      directionX="right"
                      className="menu-button"
                      isDisabled
                    />
                  </div>
                )}
              </div>
              <div className="tile-tag border-color">
                <Loaders.Rectangle
                  animate={false}
                  width="63"
                  height="24"
                  className="loaders-theme"
                />
              </div>
            </div>

            {isViewTablet && (
              <div className="tile background border-color tile-half">
                <div className="tile-name">
                  <div className="tile-container">
                    <div className="tile-icon">
                      <Loaders.Rectangle
                        animate={false}
                        width="32"
                        height="32"
                        className="loaders-theme"
                      />
                    </div>

                    <div className="tile-title">
                      <Loaders.Rectangle
                        animate={false}
                        width="48"
                        height="10"
                        className="loaders-theme"
                      />
                    </div>
                  </div>
                </div>
                <div className="tile-tag border-color">
                  <Loaders.Rectangle
                    animate={false}
                    width="63"
                    height="24"
                    className="loaders-theme"
                  />
                </div>
              </div>
            )}
          </div>

          <div className="section-flex-tablet">
            <div className="tile-name only-tile-name background border-color">
              <div className="tile-container">
                <div className="tile-icon">
                  <Loaders.Rectangle
                    animate={false}
                    width="32"
                    height="32"
                    className="loaders-theme"
                  />
                </div>

                <div className="tile-title">
                  <Loaders.Rectangle
                    animate={false}
                    width="48"
                    height="10"
                    className="loaders-theme"
                  />
                </div>
              </div>

              <div className="action-button">
                {!isViewTablet && (
                  <svg
                    className="pin"
                    width="12"
                    height="16"
                    viewBox="0 0 12 16"
                    fill="none"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path d="M9.5783 -0.000242493L2.41936 -0.000241966C2.1608 -0.000374392 1.95098 0.209442 1.95111 0.468004L1.95111 0.498338C1.95105 0.937244 2.12199 1.35006 2.43234 1.66041C2.63229 1.86036 2.87496 2.00143 3.13948 2.07719L2.60851 7.27556C2.06569 7.30602 1.55963 7.53167 1.17212 7.91918C0.754536 8.33676 0.524586 8.8919 0.524652 9.48234L0.524652 9.52725C0.524586 9.78587 0.73427 9.99556 0.992898 9.99549L4.99937 9.99549L4.9993 13.5101C5.0005 13.5949 5.17017 15.0259 5.19137 15.2188C5.21258 15.4118 5.36324 15.5487 5.36417 15.5624C5.38013 15.8082 5.75521 15.9992 6.00178 15.9998C6.13093 16 6.41664 15.9478 6.50187 15.8625C6.57956 15.7848 6.63029 15.6798 6.63818 15.5624C6.6389 15.5521 6.79437 15.3697 6.81097 15.2188C6.82757 15.068 7.00165 13.595 7.00284 13.5036V9.99562L11.0049 9.99562C11.1342 9.99569 11.2513 9.94324 11.336 9.85853C11.4207 9.77382 11.4733 9.65666 11.4731 9.52738V9.48247C11.4732 8.89203 11.2432 8.33683 10.8257 7.91931C10.4382 7.5318 9.93203 7.30622 9.38928 7.27569L8.85831 2.07733C9.12283 2.00156 9.3655 1.86049 9.56545 1.66054C9.87587 1.35012 10.0468 0.937442 10.0467 0.49847L10.0467 0.468136C10.0466 0.209376 9.83692 -0.000308579 9.5783 -0.000242493Z" />
                  </svg>
                )}
                <ContextMenuButton
                  getData={() => {}}
                  directionX="right"
                  className="menu-button"
                  isDisabled
                />
              </div>
            </div>

            {isViewTablet && (
              <div className="tile-name only-tile-name background border-color tablet-tile-name">
                <div className="tile-container">
                  <div className="tile-icon">
                    <Loaders.Rectangle
                      animate={false}
                      width="32"
                      height="32"
                      className="loaders-theme"
                    />
                  </div>

                  <div className="tile-title">
                    <Loaders.Rectangle
                      animate={false}
                      width="48"
                      height="10"
                      className="loaders-theme"
                    />
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>

        {isViewTablet && (
          <StyledFloatingButton
            className={floatingButtonClass}
            colorPreview={colorPreview}
            themePreview={themePreview}
            selectThemeId={selectThemeId}
          >
            <IconBox
              colorPreview={colorPreview}
              themePreview={themePreview}
              selectThemeId={selectThemeId}
              colorCheckImg={colorCheckImg}
            >
              <ButtonPlusIcon />
            </IconBox>
          </StyledFloatingButton>
        )}
      </div>
    </StyledComponent>
  );
};

Preview.defaultProps = {
  withBorder: true,
  withTileActions: true,
};

export default Preview;
