const getPlugins = async () => {
  const plugins = await (
    await fetch("http://localhost:3000/api/2.0/plugins")
  ).json();

  return plugins;
};

const activatePlugin = async (id) => {
  const plugin = await (
    await fetch(`http://localhost:3000/api/2.0/plugins/activate/${id}`, {
      method: "PUT",
    })
  ).json();

  return plugin;
};

const uploadPlugin = async (formData) => {
  const plugin = await (
    await fetch("http://localhost:3000/api/2.0/plugins/upload", {
      method: "POST",
      body: formData,
    })
  ).json();

  return plugin;
};

const deletePlugin = async (id) => {
  await fetch(`http://localhost:3000/api/2.0/plugins/delete/${id}`, {
    method: "DELETE",
    body: {},
  });
};

export default { getPlugins, activatePlugin, uploadPlugin, deletePlugin };
