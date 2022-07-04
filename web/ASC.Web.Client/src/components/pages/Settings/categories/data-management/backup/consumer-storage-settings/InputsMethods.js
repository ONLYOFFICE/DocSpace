export const onChangeTextInput = (formSettings, event) => {
  const { target } = event;
  const value = target.value;
  const name = target.name;
  return { ...formSettings, [name]: value };
};

export const onChangeCheckbox = (formSettings, event) => {
  const { target } = event;
  const value = target.checked;
  const name = target.name;
  return { ...formSettings, [name]: value };
};

export const onSelectEncryptionMode = (formSettings, name, nonCheckName) => {
  return {
    ...formSettings,
    [name]: true,
    [nonCheckName]: false,
  };
};
export const onSetAdditionInfo = (formSettings, name, value) => {
  return { ...formSettings, [name]: value };
};
