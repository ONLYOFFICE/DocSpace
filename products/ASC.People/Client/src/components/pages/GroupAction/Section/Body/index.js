import React, { useCallback, useState, useEffect } from 'react';
import { withRouter } from 'react-router';
import PropTypes from "prop-types";
import {
  Button,
  TextInput,
  Text,
  InputBlock,
  Icons,
  SelectedItem
} from "asc-web-components";
import { useTranslation } from 'react-i18next';
import { department, headOfDepartment, typeUser } from '../../../../../helpers/customNames';
import { connect } from 'react-redux';
import { resetGroup } from '../../../../../store/group/actions';

const SectionBodyContent = (props) => {
  const { history, group, resetGroup } = props;
  const [value, setValue] = useState(group ? group.name : "");
  const [error, setError] = useState(null);
  const [inLoading, setInLoading] = useState(false);
  const { t } = useTranslation();

  useEffect(() => {
    setValue(group ? group.name : "");
    setError(null);
    setInLoading(false);
  }, [group]);

  const groupMembers = group && group.members ? group.members : [];

  const onCancel = useCallback(() => {
    resetGroup();
    history.goBack();
  }, [history, resetGroup]);

  const onChange = useCallback((e) => setValue(e.target.value), [setValue]);

  console.log("Group render", props);

  return (
    <>
      <div>
        <label htmlFor="group-name">
          <Text.Body as="span" isBold={true}>{t('CustomDepartmentName', { department })}:</Text.Body>
        </label>
        <div style={{width: "320px"}}>
          <TextInput id="group-name" name="group-name" scale={true} isAutoFocussed={true} tabIndex={1} value={value} onChange={onChange} />
        </div>
      </div>
      <div style={{ marginTop: "16px" }}>
        <label htmlFor="head-selector">
          <Text.Body as="span" isBold={true}>{t('CustomHeadOfDepartment', { headOfDepartment })}:</Text.Body>
        </label>
        <InputBlock
          id="head-selector"
          value={t('CustomAddEmployee', { typeUser })}
          iconName="ExpanderDownIcon"
          iconSize={8}
          isIconFill={true}
          iconColor="#A3A9AE"
          scale={false}
          isReadOnly={true}
          tabIndex={2}
        >
          <Icons.CatalogEmployeeIcon size="medium" />
        </InputBlock>
      </div>
      <div style={{ marginTop: "16px" }}>
        <label htmlFor="employee-selector">
          <Text.Body as="span" isBold={true}>Members:</Text.Body>
        </label>
        <InputBlock
          id="employee-selector"
          value={t('CustomAddEmployee', { typeUser })}
          iconName="ExpanderDownIcon"
          iconSize={8}
          isIconFill={true}
          iconColor="#A3A9AE"
          scale={false}
          isReadOnly={true}
          tabIndex={3}
        >
          <Icons.CatalogGuestIcon size="medium" />
        </InputBlock>
      </div>
      <div style={{ marginTop: "16px", display: "flex", flexWrap: "wrap", flexDirection: "row" }}>
        {groupMembers.map(member => 
          <SelectedItem
            key={member.id}
            text={member.displayName}
            onClick={(e) => console.log("onClick", e.target)}
            onClose={(e) => console.log("onClose", e.target)}
            isInline={true}
            style={{ marginRight: "8px", marginBottom: "8px" }}
          />
        )}
      </div>
      <div>{error && <strong>{error}</strong>}</div>
      <div style={{ marginTop: "60px" }}>
        <Button label={t('SaveButton')} primary type="submit" isDisabled={inLoading} size="big" tabIndex={4} />
        <Button
          label={t('CancelButton')}
          style={{ marginLeft: "8px" }}
          size="big"
          isDisabled={inLoading}
          onClick={onCancel}
          tabIndex={5}
        />
      </div>
    </>
  );
};

SectionBodyContent.propTypes = {
  group: PropTypes.object
};

SectionBodyContent.defaultProps = {
  group: null
}

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup
  };
};

export default connect(mapStateToProps, { resetGroup })(withRouter(SectionBodyContent));