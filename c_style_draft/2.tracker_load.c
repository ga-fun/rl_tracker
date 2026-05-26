#include "tracker.h"
#include <stdbool.h>
#include <stddef.h>
#include <stdlib.h>
#include <string.h>

#define DEFAULT_EPIC_DIR	""	// TODO
#define DEFAULT_STEAM_DIR	""	// TODO
#define INI_FILE_PATH 		"\\TAGame\\Config\\DefaultStatsAPI.ini"

static char	*build_ini_file_path(const char *install_dir)
{
	size_t	dir_len;
	size_t	file_len;
	char	*path;

	if (!install_dir)
		return (false);
	dir_len = strlen(install_dir);
	file_len = strlen(INI_FILE_PATH);
	path = malloc(dir_len + file_len + 1);
	if (!path)
		return (NULL);
	strncpy(path, install_dir, dir_len);
	strlcat(path, INI_FILE_PATH, file_len);
	return (path);
}

static bool	is_ini_file_ready(const char *file_path, int port, float packet_send_rate)
{
	// TODO: check if file exists
	// TODO: check if port has the correct value
	// TODO: check if packet_send_rate has the correct value
}

void	tracker_load(t_state *state)
{
	//
}
