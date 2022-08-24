export const getPlugins = async () => {
  const plugins = await (
    await fetch("http://localhost:8092/api/2.0/plugins")
  ).json();

  return plugins;
};

export const activatePlugin = async (id) => {
  const plugin = await (
    await fetch(`http://localhost:8092/api/2.0/plugins/activate/${id}`, {
      method: "PUT",
    })
  ).json();

  return plugin;
};

export const uploadPlugin = async (formData) => {
  const plugin = await (
    await fetch("http://localhost:8092/api/2.0/plugins/upload", {
      method: "POST",
      body: formData,
    })
  ).json();

  return plugin;
};

export const deletePlugin = async (id) => {
  await fetch(`http://localhost:8092/api/2.0/plugins/delete/${id}`, {
    method: "DELETE",
    body: {},
  });
};
