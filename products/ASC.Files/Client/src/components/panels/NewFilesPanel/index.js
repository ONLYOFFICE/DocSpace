import React from "react";
import PropTypes from "prop-types";
import { connect } from "react-redux";
import { withRouter } from "react-router";
import {
  Backdrop,
  Heading,
  Aside,
  Row,
  RowContent,
  RowContainer,
  Text,
  Link,
  Button
} from "asc-web-components";
import { withTranslation } from "react-i18next";
import { utils as commonUtils } from "asc-web-common";
import i18n from "./i18n";
import { ReactSVG } from 'react-svg'
import {
  StyledAsidePanel,
  StyledContent,
  StyledHeaderContent,
  StyledBody,
  StyledFooter
} from "../StyledPanels";
import { getFileIcon, getFolderIcon } from '../../../store/files/selectors';

const { changeLanguage } = commonUtils;

class NewFilesPanelComponent extends React.Component {
  constructor(props) {
    super(props);

    changeLanguage(i18n);
  }

    getItemIcon = (item, isEdit) => {
    const extension = item.fileExst;
    const icon = extension
      ? getFileIcon(extension, 24)
      : getFolderIcon(item.providerKey, 24);

    return <ReactSVG
      beforeInjection={svg => {
        svg.setAttribute('style', 'margin-top: 4px');
        isEdit && svg.setAttribute('style', 'margin-left: 24px');
      }}
      src={icon}
      loading={this.svgLoader}
    />;
  };

  render() {
    //console.log("NewFiles panel render");
    const { t, visible, onClose, files } = this.props;
    const zIndex = 310;

    return (
      <StyledAsidePanel visible={visible}>
        <Backdrop onClick={onClose} visible={visible} zIndex={zIndex} />
        <Aside className="header_aside-panel" visible={visible}>
          <StyledContent>
            <StyledHeaderContent className="files-operations-panel">
              <Heading size="medium" truncate>
                {t("NewFiles")}
              </Heading>
            </StyledHeaderContent>
            <StyledBody className="files-operations-body">
              <RowContainer useReactWindow manualHeight="83vh">
                {files.map((file) => {
                  const element = this.getItemIcon(file);
                  return (
                    <Row key={file.id} element={element}>
                      <RowContent>
                        <Link
                          containerWidth="100%"
                          type="page"
                          fontWeight="bold"
                          color="#333"
                          isTextOverflow
                          truncate
                          title={file.title}
                          fontSize="14px"
                        >
                          {file.title}
                        </Link>

                        <></>
                        <Text fontSize="12px" containerWidth="auto">
                          {file.checked && t("ConvertInto")}
                        </Text>
                      </RowContent>
                    </Row>
                  );
                })}
              </RowContainer>
            </StyledBody>
            <StyledFooter>
              <Button
                label={t("MarkAsRead")}
                size="big"
                primary
                //onClick={this.onSaveClick}
              />
                <Button
                className="sharing_panel-button"
                label={t("CloseButton")}
                size="big"
                onClick={onClose}
              />
            </StyledFooter>
          </StyledContent>
        </Aside>
      </StyledAsidePanel>
    );
  }
}

NewFilesPanelComponent.propTypes = {
  onClose: PropTypes.func,
  visible: PropTypes.bool,
};

const NewFilesPanelContainerTranslated = withTranslation()(
  NewFilesPanelComponent
);

const NewFilesPanel = (props) => (
  <NewFilesPanelContainerTranslated i18n={i18n} {...props} />
);

const mapStateToProps = (state) => {
  return {};
};

export default connect(mapStateToProps, {})(withRouter(NewFilesPanel));
